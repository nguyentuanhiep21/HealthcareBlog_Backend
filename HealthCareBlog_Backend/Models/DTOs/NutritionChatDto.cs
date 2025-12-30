namespace HealthCareBlog_Backend.Models.DTOs
{
    public class MealDto
    {
        public string Name { get; set; } = string.Empty;
        public int Calories { get; set; }
        public List<string> Items { get; set; } = new();
        public string Icon { get; set; } = string.Empty;
    }

    public class SaveChatMessageDto
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<MealDto>? ParsedMeals { get; set; }
    }

    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<MealDto>? ParsedMeals { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ChatSessionDto
    {
        public int Id { get; set; }
        public int NutritionProfileId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ChatMessageDto> Messages { get; set; } = new();
    }

    public class NutritionDataDto
    {
        public NutritionProfileDto? Profile { get; set; }
        public ChatSessionDto? ActiveSession { get; set; }
    }
}
