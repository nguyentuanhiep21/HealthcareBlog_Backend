namespace HealthCareBlog_Backend.Services.Interfaces;

public interface IPresenceTracker
{
    Task<bool> UserConnected(string userId, string connectionId);
    Task<bool> UserDisconnected(string userId, string connectionId);
    Task<string[]> GetOnlineUsers();
    Task<bool> IsUserOnline(string userId);
}
