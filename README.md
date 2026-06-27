# Healthcare Blog - Full Stack Application

Ứng dụng blog chăm sóc sức khỏe với Next.js frontend và .NET backend.

## 🚀 Quick Start

### Development (Local)

1. **Khởi động Backend**:
   ```powershell
   cd HealthCareBlog_Backend\HealthCareBlog_Backend
   dotnet run
   ```
   Backend chạy trên: `https://localhost:7223`

2. **Khởi động Frontend**:
   ```powershell
   cd HealthcareBlog_Frontend
   pnpm dev
   ```
   Frontend chạy trên: `http://localhost:3000`

## 🔧 Tech Stack

### Frontend
- Next.js 16.0.3
- React 19
- TypeScript
- Tailwind CSS
- Radix UI

### Backend
- .NET 8.0
- Entity Framework Core
- SQL Server
- JWT Authentication

## ⚙️ Configuration

### Frontend Environment Variables (.env.local)

```env
NEXT_PUBLIC_API_URL=https://localhost:7223
NEXT_PUBLIC_FRONTEND_URL=http://localhost:3000
```

### Backend Configuration

Xem [appsettings.json](HealthCareBlog_Backend/HealthCareBlog_Backend/appsettings.json) để cấu hình:
- Database connection string
- JWT settings
- CORS origins
- Email configuration

```appsettings
{
  "ConnectionStrings": {
    "DefaultSQLConnection": ""
  },
  "JwtSettings": {
    "SecretKey": "",
    "Issuer": "HealthCareBlog",
    "Audience": "HealthCareBlogUsers",
    "ExpirationDays": 7
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "",
    "SmtpPassword": "",
    "FromEmail": "",
    "FromName": "HealthCareBlog"
  },
  "AppSettings": {
    "FrontendUrl": "http://localhost:3000"
}
```

## 🧠 Core Logic: Meal Suggestion Flow
Chức năng "Gợi ý thực đơn":

1. **AI Health Assessment (`POST /api/HealthAssessment/assess`)**
   - Sử dụng **Machine Learning Model** (`health_model.onnx` - TreeEnsembleRegressor).
   - Nhận 5 tham số đầu vào của user: `Gender`, `Age`, `Height`, `Weight`, `Goal`.
   - Dự đoán chính xác lượng dinh dưỡng mục tiêu: `Calories`, `Protein`, `Carbs`, `Fat`.

2. **Meal Suggestion (`POST /api/MealSuggestion/recommend`)**
   - Dựa trên lượng `Calories` mục tiêu từ AI, tự động phân bổ theo các bữa: Sáng (25%), Trưa (35%), Tối (30%), 2 Bữa Phụ (Mỗi bữa 5%).
   - Query lấy món ăn ngẫu nhiên (`ORDER BY RANDOM()`) từ Database khớp với mục tiêu (`goal`) và loại bữa ăn (`mealType`), với biên độ Calo cho phép từ `±15%` đến `±30%`.
