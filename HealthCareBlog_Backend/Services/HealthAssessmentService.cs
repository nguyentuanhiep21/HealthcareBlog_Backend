using HealthCareBlog_Backend.Models.DTOs;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace HealthCareBlog_Backend.Services
{
    /// <summary>
    /// C# reimplementation của health_assessor.py (testrcm).
    /// Dùng công thức Mifflin-St Jeor + macro ratios cố định — không cần Python runtime.
    /// Hoạt động hoàn toàn trên Render (không phụ thuộc external service).
    /// </summary>
    public class HealthAssessmentService : IHealthAssessmentService
    {
        private static readonly string _modelPath = Path.Combine(AppContext.BaseDirectory, "Models", "ML", "health_model.onnx");
        private static readonly Lazy<InferenceSession> _session = new(() => new InferenceSession(_modelPath));

        // ── Mapping tables ────────────────────────────────────────────────────

        private static readonly Dictionary<string, string> _genderMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Male"]  = "Male",
            ["Female"]= "Female",
        };

        private static readonly Dictionary<string, string> _goalMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Gain_Muscle"] = "Gain_Muscle",
            ["Lose_Fat"]    = "Lose_Fat",
            ["Maintain"]    = "Maintain",
        };

        // BMI table: (low, high, label)
        private static readonly (float Lo, float Hi, string Label)[] _bmiTable =
        [
            (0f,    18.5f, "Gầy (Thiếu cân)"),
            (18.5f, 23.0f, "Bình thường"),
            (23.0f, 27.5f, "Thừa cân"),
            (27.5f, 35.0f, "Béo phì độ I"),
            (35.0f, float.MaxValue, "Béo phì độ II+"),
        ];

        // Advice table (goal × bmi_category)
        private static readonly Dictionary<string, Dictionary<string, string>> _advice = new()
        {
            ["Lose_Fat"] = new()
            {
                ["Gầy (Thiếu cân)"] = "Bạn đang thiếu cân, hãy ưu tiên ăn đủ chất hơn là giảm mỡ.",
                ["Bình thường"]     = "Cân nặng của bạn đang ổn. Tập luyện nhẹ và duy trì chế độ ăn cân bằng.",
                ["Thừa cân"]        = "Giảm mỡ là mục tiêu hợp lý. Kiểm soát carb và tăng protein.",
                ["Béo phì độ I"]    = "Cần giảm mỡ nghiêm túc. Hạn chế đường, ăn nhiều rau xanh và protein.",
                ["Béo phì độ II+"]  = "Nên tham khảo bác sĩ/chuyên gia dinh dưỡng trước khi thực hiện chế độ ăn.",
            },
            ["Gain_Muscle"] = new()
            {
                ["Gầy (Thiếu cân)"] = "Tập luyện kháng lực và ăn dư calo 200–300 kcal/ngày để tăng cơ hiệu quả.",
                ["Bình thường"]     = "Tình trạng lý tưởng để tăng cơ. Tăng protein và tập nặng dần.",
                ["Thừa cân"]        = "Nên giảm mỡ trước rồi mới tập tăng cơ để hiệu quả hơn.",
                ["Béo phì độ I"]    = "Ưu tiên giảm mỡ trước khi nghĩ đến tăng cơ.",
                ["Béo phì độ II+"]  = "Nên tham khảo bác sĩ trước khi bắt đầu chế độ tập luyện tăng cơ.",
            },
            ["Maintain"] = new()
            {
                ["Gầy (Thiếu cân)"] = "Duy trì không phù hợp khi đang thiếu cân. Hãy cân nhắc mục tiêu tăng cơ.",
                ["Bình thường"]     = "Tuyệt vời! Duy trì thói quen ăn uống và luyện tập hiện tại.",
                ["Thừa cân"]        = "Bạn nên cân nhắc mục tiêu giảm mỡ thay vì chỉ duy trì.",
                ["Béo phì độ I"]    = "Duy trì không phù hợp. Cần giảm cân vì lý do sức khỏe.",
                ["Béo phì độ II+"]  = "Nên tham khảo bác sĩ về kế hoạch sức khỏe phù hợp.",
            },
        };

        // ── Public method ─────────────────────────────────────────────────────

        public (HealthAssessResultDto? result, string? error) Assess(HealthAssessRequestDto req)
        {
            if (!_genderMap.TryGetValue(req.Gender?.Trim() ?? "", out var gender))
                return (null, $"gender không hợp lệ: '{req.Gender}'. Hợp lệ: Male, Female.");

            if (!_goalMap.TryGetValue(req.Goal?.Trim() ?? "", out var goal))
                return (null, $"goal không hợp lệ: '{req.Goal}'. Hợp lệ: Gain_Muscle, Lose_Fat, Maintain.");

            if (req.Age is < 10 or > 100)
                return (null, $"age phải trong khoảng 10–100, nhận được: {req.Age}.");

            if (req.Weight is < 30 or > 300)
                return (null, $"weight phải trong khoảng 30–300 kg, nhận được: {req.Weight}.");

            if (req.Height is < 100 or > 250)
                return (null, $"height phải trong khoảng 100–250 cm, nhận được: {req.Height}.");

            // 2. ML Model Inference via ONNX
            float genderVal = gender == "Male" ? 1f : 0f;
            float goalVal = goal switch { "Lose_Fat" => 0f, "Maintain" => 1f, "Gain_Muscle" => 2f, _ => 1f };

            var inputTensor = new DenseTensor<float>(new[] { genderVal, (float)req.Age, req.Height, req.Weight, goalVal }, new[] { 1, 5 });
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("float_input", inputTensor) };
            
            using var results = _session.Value.Run(inputs);
            var output = results.First().AsEnumerable<float>().ToArray();
            
            int calories = (int)Math.Round(output[0]);
            int protein  = (int)Math.Round(output[1]);
            int carbs    = (int)Math.Round(output[2]);
            int fat      = (int)Math.Round(output[3]);

            // 6. BMI
            float heightM = req.Height / 100f;
            float bmi     = MathF.Round(req.Weight / (heightM * heightM), 2);
            string bmiCategory = GetBmiCategory(bmi);

            // 7. Health Score (0–100)
            int healthScore = ComputeHealthScore(bmi, bmiCategory, goal);

            // 8. Advice
            string advice = _advice.GetValueOrDefault(goal, new())
                                    .GetValueOrDefault(bmiCategory, string.Empty);

            return (new HealthAssessResultDto
            {
                Status      = "ok",
                Input = new
                {
                    gender  = req.Gender,
                    age     = req.Age,
                    height  = req.Height,
                    weight  = req.Weight,
                    goal    = req.Goal,
                },
                Bmi         = bmi,
                BmiCategory = bmiCategory,
                HealthScore = healthScore,
                Nutrition   = new NutritionTargetDto
                {
                    CaloriesKcal = calories,
                    ProteinG     = protein,
                    CarbsG       = carbs,
                    FatG         = fat,
                },
                Advice = advice,
            }, null);
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private static string GetBmiCategory(float bmi)
        {
            foreach (var (lo, hi, label) in _bmiTable)
                if (bmi >= lo && bmi < hi) return label;
            return "Không xác định";
        }

        /// <summary>
        /// Health Score 0–100 dựa trên khoảng cách BMI đến vùng lý tưởng (18.5–23).
        /// Penalty nếu goal không phù hợp BMI.
        /// (Mirror của _compute_health_score trong health_assessor.py)
        /// </summary>
        private static int ComputeHealthScore(float bmi, string bmiCategory, string goal)
        {
            const float idealLo = 18.5f, idealHi = 23.0f;

            double bmiScore;
            if (bmi >= idealLo && bmi <= idealHi)
                bmiScore = 80;
            else if (bmi < idealLo)
                bmiScore = Math.Max(0, 80 - (idealLo - bmi) * 5);
            else
                bmiScore = Math.Max(0, 80 - (bmi - idealHi) * 4);

            double alignmentScore = 20;
            if (bmi < 18.5f && goal == "Lose_Fat")    alignmentScore = 0;
            else if (bmi >= 27.5f && goal == "Gain_Muscle") alignmentScore = 5;

            return Math.Min(100, (int)Math.Round(bmiScore + alignmentScore));
        }
    }
}
