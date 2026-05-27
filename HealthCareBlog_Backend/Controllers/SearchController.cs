using HealthCareBlog_Backend.Extensions;
using HealthCareBlog_Backend.Models.DTOs.Search;
using HealthCareBlog_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareBlog_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<ActionResult<SearchResultDTO>> SearchAll(
        [FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetUserId();
        var results = await _searchService.SearchAllAsync(userId, query, page, pageSize);
        return Ok(results);
    }

    [HttpGet("posts")]
    public async Task<ActionResult<List<SearchPostResultDTO>>> SearchPosts(
        [FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetUserId();
        var posts = await _searchService.SearchPostsAsync(userId, query, page, pageSize);
        return Ok(posts);
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<SearchUserResultDTO>>> SearchUsers(
        [FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetUserId();
        var users = await _searchService.SearchUsersAsync(userId, query, page, pageSize);
        return Ok(users);
    }
}
