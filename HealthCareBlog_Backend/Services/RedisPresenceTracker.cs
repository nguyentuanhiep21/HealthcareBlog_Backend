using HealthCareBlog_Backend.Services.Interfaces;
using StackExchange.Redis;

namespace HealthCareBlog_Backend.Services;

public class RedisPresenceTracker : IPresenceTracker
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    
    // Hash key lưu trạng thái: Field = userId, Value = count of connections
    private const string PresenceKey = "healthcareblog:presence";

    public RedisPresenceTracker(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public async Task<bool> UserConnected(string userId, string connectionId)
    {
        // Tăng số lượng kết nối của user lên 1
        var count = await _db.HashIncrementAsync(PresenceKey, userId, 1);
        return count == 1; // True nếu đây là kết nối đầu tiên (vừa chuyển sang online)
    }

    public async Task<bool> UserDisconnected(string userId, string connectionId)
    {
        // Giảm số lượng kết nối của user đi 1
        var count = await _db.HashDecrementAsync(PresenceKey, userId, 1);
        
        // Nếu không còn kết nối nào, xóa user khỏi hash
        if (count <= 0)
        {
            await _db.HashDeleteAsync(PresenceKey, userId);
            return true; // True nếu đây là kết nối cuối cùng (vừa chuyển sang offline)
        }
        return false;
    }

    public async Task<string[]> GetOnlineUsers()
    {
        var users = await _db.HashKeysAsync(PresenceKey);
        return users.Select(k => k.ToString()).ToArray();
    }

    public async Task<bool> IsUserOnline(string userId)
    {
        return await _db.HashExistsAsync(PresenceKey, userId);
    }
}
