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

        // Sau đó lặp lại mỗi 1 tiếng
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        
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
            
            var lastHour = DateTime.UtcNow.AddHours(-1);

            // Chỉ lấy Id, không kéo toàn bộ object (giảm tải memory & DB IO)
            var trendingPostIds = await dbContext.Posts
                .Where(p => p.CreatedAt >= lastHour)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            if (trendingPostIds.Count < 30)
            {
                var needed = 30 - trendingPostIds.Count;
                // Bù bài mới nhất nếu chưa đủ 30 bài
                var recentPosts = await dbContext.Posts
                    .Where(p => !trendingPostIds.Contains(p.Id))
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => p.Id)
                    .Take(needed)
                    .ToListAsync(cancellationToken);

                trendingPostIds.AddRange(recentPosts);
            }

            // Lấy đủ 30 bài và tính Like + Comment để tìm Top 3
            var top3Ids = await dbContext.Posts
                .Where(p => trendingPostIds.Contains(p.Id))
                .Select(p => new { p.Id, p.LikeCount, p.CommentCount })
                .ToListAsync(cancellationToken);
                
            var finalTrendingIds = top3Ids
                .OrderByDescending(p => p.LikeCount + p.CommentCount)
                .Take(3)
                .Select(p => p.Id)
                .ToList();

            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(finalTrendingIds);
            
            // Lưu vào Redis (TTL 65 phút để phòng hờ timer delay)
            await db.StringSetAsync(TrendingPostsRedisKey, json, TimeSpan.FromMinutes(65));
            
            _logger.LogInformation($"Updated Trending Posts in Redis. IDs: {string.Join(", ", finalTrendingIds)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating trending posts cache.");
        }
    }
}
