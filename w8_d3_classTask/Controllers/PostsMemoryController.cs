using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace w8_d3_classTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsMemoryController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PostsController> _logger;


        public PostsMemoryController(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<PostsController> logger)
        {

            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _logger = logger;


        }


        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            string cacheKey = "posts";

            // Try to get data from the in-memory cache
            if (_memoryCache.TryGetValue(cacheKey, out object? cachedPosts))
            {
                _logger.LogInformation("Data retrieved from IMemoryCache");
                return Ok(cachedPosts);
            }

            // If not found in cache, fetch from API
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch from external API.");

            var json = await response.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<object>(json);

            // Set data in cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _memoryCache.Set(cacheKey, posts, cacheEntryOptions);
            _logger.LogInformation("Data fetched from API and cached in memory");

            return Ok(posts);
        }
    }


}

