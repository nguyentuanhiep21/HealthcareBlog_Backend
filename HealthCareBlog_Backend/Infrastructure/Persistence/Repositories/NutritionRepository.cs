using HealthCareBlog_Backend.Application.Interfaces.Repositories;
using HealthCareBlog_Backend.Data;
using HealthCareBlog_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCareBlog_Backend.Infrastructure.Persistence.Repositories;

public class NutritionRepository : INutritionRepository
{
    private readonly ApplicationDbContext _context;

    public NutritionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<NutritionProfile?> GetProfileByUserIdAsync(string userId)
        => await _context.NutritionProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

    public async Task AddProfileAsync(NutritionProfile profile)
        => await _context.NutritionProfiles.AddAsync(profile);

    public async Task<NutritionChatSession?> GetSessionByProfileIdAsync(int profileId)
        => await _context.NutritionChatSessions
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.NutritionProfileId == profileId);

    public async Task<NutritionChatSession?> GetActiveSessionWithMessagesAsync(string userId)
        => await _context.NutritionChatSessions
            .Include(s => s.NutritionProfile)
            .Include(s => s.Messages.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(s => s.NutritionProfile.UserId == userId);

    public async Task<NutritionChatSession?> GetSessionWithProfileAsync(string userId)
        => await _context.NutritionChatSessions
            .Include(s => s.NutritionProfile)
            .FirstOrDefaultAsync(s => s.NutritionProfile.UserId == userId);

    public async Task AddSessionAsync(NutritionChatSession session)
        => await _context.NutritionChatSessions.AddAsync(session);

    public void RemoveSession(NutritionChatSession session)
        => _context.NutritionChatSessions.Remove(session);

    public async Task AddMessageAsync(NutritionChatMessage message)
        => await _context.NutritionChatMessages.AddAsync(message);

    public async Task<List<NutritionChatMessage>> GetMessagesBySessionIdAsync(int sessionId)
        => await _context.NutritionChatMessages
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

    public Task<int> SaveChangesAsync()
        => _context.SaveChangesAsync();
}
