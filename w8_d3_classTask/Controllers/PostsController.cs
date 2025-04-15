using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace w8_d3_classTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        //inject nedded services 
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _cache;
        private readonly ILogger<PostsController> _logger;


        public PostsController(IHttpClientFactory httpClientFactory, IDistributedCache cache, ILogger<PostsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            string cacheKey = "posts";
            string? cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Data from Redis Cache");
                var cachedPosts = JsonSerializer.Deserialize<object>(cachedData);
                return Ok(cachedPosts);
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch from external API.");

            var json = await response.Content.ReadAsStringAsync();
            await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // cache expires in 5 mins
            });

            _logger.LogInformation("Data fetched from API and cached in Redis");
            var posts = JsonSerializer.Deserialize<object>(json);
            return Ok(posts);
        }
    }
}
