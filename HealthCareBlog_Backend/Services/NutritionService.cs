using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.DTOs;
using HealthCareBlog_Backend.Models.Entities;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HealthCareBlog_Backend.Services
{
    public class NutritionService : INutritionService
    {
        private readonly ApplicationDbContext _context;

        public NutritionService(ApplicationDbContext context)
        {
            _context = context;
        }

        private decimal CalculateBMI(decimal height, decimal weight)
        {
            return Math.Round(weight / (height / 100 * height / 100), 1);
        }

        private string GetBMIStatus(decimal bmi)
        {
            if (bmi < 18.5m) return "Gầy";
            if (bmi < 25m) return "Bình thường";
            if (bmi < 30m) return "Thừa cân";
            return "Béo phì";
        }

        public async Task<NutritionProfileDto?> GetUserProfileAsync(string userId)
        {
            var profile = await _context.NutritionProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null) return null;

            return new NutritionProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                Gender = profile.Gender,
                Age = profile.Age,
                Height = profile.Height,
                Weight = profile.Weight,
                BMI = profile.BMI,
                BMIStatus = profile.BMIStatus,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }

        public async Task<NutritionProfileDto> CreateOrUpdateProfileAsync(string userId, CreateNutritionProfileDto dto)
        {
            var profile = await _context.NutritionProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            var bmi = CalculateBMI(dto.Height, dto.Weight);
            var bmiStatus = GetBMIStatus(bmi);

            if (profile == null)
            {
                // Create new profile
                profile = new NutritionProfile
                {
                    UserId = userId,
                    Gender = dto.Gender,
                    Age = dto.Age,
                    Height = dto.Height,
                    Weight = dto.Weight,
                    BMI = bmi,
                    BMIStatus = bmiStatus,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.NutritionProfiles.Add(profile);
            }
            else
            {
                // Update existing profile
                profile.Gender = dto.Gender;
                profile.Age = dto.Age;
                profile.Height = dto.Height;
                profile.Weight = dto.Weight;
                profile.BMI = bmi;
                profile.BMIStatus = bmiStatus;
                profile.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return new NutritionProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                Gender = profile.Gender,
                Age = profile.Age,
                Height = profile.Height,
                Weight = profile.Weight,
                BMI = profile.BMI,
                BMIStatus = profile.BMIStatus,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }

        public async Task<NutritionDataDto> GetUserNutritionDataAsync(string userId)
        {
            var profile = await GetUserProfileAsync(userId);
            
            if (profile == null)
            {
                return new NutritionDataDto
                {
                    Profile = null,
                    ActiveSession = null
                };
            }

            var session = await GetActiveSessionAsync(userId);

            return new NutritionDataDto
            {
                Profile = profile,
                ActiveSession = session
            };
        }

        public async Task<ChatSessionDto> CreateNewSessionAsync(string userId, CreateNutritionProfileDto profileDto)
        {
            // Create or update profile
            var profile = await CreateOrUpdateProfileAsync(userId, profileDto);

            // Delete old session if exists
            var oldSession = await _context.NutritionChatSessions
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.NutritionProfileId == profile.Id);

            if (oldSession != null)
            {
                _context.NutritionChatSessions.Remove(oldSession);
                await _context.SaveChangesAsync();
            }

            // Create new session
            var newSession = new NutritionChatSession
            {
                NutritionProfileId = profile.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.NutritionChatSessions.Add(newSession);
            await _context.SaveChangesAsync();

            return new ChatSessionDto
            {
                Id = newSession.Id,
                NutritionProfileId = newSession.NutritionProfileId,
                CreatedAt = newSession.CreatedAt,
                UpdatedAt = newSession.UpdatedAt,
                Messages = new List<ChatMessageDto>()
            };
        }

        public async Task<ChatSessionDto?> GetActiveSessionAsync(string userId)
        {
            var session = await _context.NutritionChatSessions
                .Include(s => s.NutritionProfile)
                .Include(s => s.Messages.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(s => s.NutritionProfile.UserId == userId);

            if (session == null) return null;

            return new ChatSessionDto
            {
                Id = session.Id,
                NutritionProfileId = session.NutritionProfileId,
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt,
                Messages = session.Messages.Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    Role = m.Role,
                    Content = m.Content,
                    ParsedMeals = string.IsNullOrEmpty(m.ParsedMealsJson) 
                        ? null 
                        : JsonSerializer.Deserialize<List<MealDto>>(m.ParsedMealsJson),
                    CreatedAt = m.CreatedAt
                }).ToList()
            };
        }

        public async Task<ChatMessageDto> SaveMessageAsync(string userId, SaveChatMessageDto dto)
        {
            var session = await _context.NutritionChatSessions
                .Include(s => s.NutritionProfile)
                .FirstOrDefaultAsync(s => s.NutritionProfile.UserId == userId);

            if (session == null)
            {
                throw new InvalidOperationException("No active chat session found for user");
            }

            var message = new NutritionChatMessage
            {
                SessionId = session.Id,
                Role = dto.Role,
                Content = dto.Content,
                ParsedMealsJson = dto.ParsedMeals != null 
                    ? JsonSerializer.Serialize(dto.ParsedMeals) 
                    : null,
                CreatedAt = DateTime.UtcNow
            };

            _context.NutritionChatMessages.Add(message);
            session.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new ChatMessageDto
            {
                Id = message.Id,
                Role = message.Role,
                Content = message.Content,
                ParsedMeals = dto.ParsedMeals,
                CreatedAt = message.CreatedAt
            };
        }

        public async Task<List<ChatMessageDto>> GetSessionMessagesAsync(int sessionId)
        {
            var messages = await _context.NutritionChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return messages.Select(m => new ChatMessageDto
            {
                Id = m.Id,
                Role = m.Role,
                Content = m.Content,
                ParsedMeals = string.IsNullOrEmpty(m.ParsedMealsJson)
                    ? null
                    : JsonSerializer.Deserialize<List<MealDto>>(m.ParsedMealsJson),
                CreatedAt = m.CreatedAt
            }).ToList();
        }
    }
}
