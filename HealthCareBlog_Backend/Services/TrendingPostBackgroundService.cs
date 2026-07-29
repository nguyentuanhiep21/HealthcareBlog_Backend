using HealthCareBlog_Backend.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace HealthCareBlog_Backend.Services;

public class TrendingPostBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TrendingPostBackgroundService> _logger;
    private readonly IConnectionMultiplexer _redis;
    
    public const string TrendingPostsRedisKey = "healthcareblog:trending_posts";

    public TrendingPostBackgroundService(
        IServiceProvider serviceProvider, 
        ILogger<TrendingPostBackgroundService> logger,
        IConnectionMultiplexer redis)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _redis = redis;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Chạy ngay lần đầu khi service khởi động
        await UpdateTrendingPostsCacheAsync(stoppingToken);

        // Sau đó lặp lại mỗi 20 phút
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(20));
        
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await UpdateTrendingPostsCacheAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("TrendingPostBackgroundService is stopping.");
        }
    }

    private async Task UpdateTrendingPostsCacheAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            // Chỉ lấy Id, không kéo toàn bộ object (giảm tải memory & DB IO)
            var trendingPostIds = await dbContext.Posts
                .Where(p => p.CreatedAt >= today && p.CreatedAt < tomorrow)
                .OrderByDescending(p => p.LikeCount + p.CommentCount)
                .Select(p => p.Id)
                .Take(3)
                .ToListAsync(cancellationToken);

            if (trendingPostIds.Count < 3)
            {
                var needed = 3 - trendingPostIds.Count;
                var extraPostIds = await dbContext.Posts
                    .Where(p => !trendingPostIds.Contains(p.Id))
                    .OrderByDescending(p => p.LikeCount + p.CommentCount)
                    .ThenByDescending(p => p.CreatedAt)
                    .Select(p => p.Id)
                    .Take(needed)
                    .ToListAsync(cancellationToken);

                trendingPostIds.AddRange(extraPostIds);
            }

            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(trendingPostIds);
            
            // Lưu vào Redis (TTL 25 phút để phòng hờ timer delay)
            await db.StringSetAsync(TrendingPostsRedisKey, json, TimeSpan.FromMinutes(25));
            
            _logger.LogInformation($"Updated Trending Posts in Redis. IDs: {string.Join(", ", trendingPostIds)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating trending posts cache.");
        }
    }
}
