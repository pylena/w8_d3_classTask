using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace w8_d3_classTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMemoryController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<UserMemoryController> _logger;

        public UserMemoryController(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<UserMemoryController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _logger = logger;

        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            string cacheKey = "users";

            // Try to get users from memory cache
            if (_memoryCache.TryGetValue(cacheKey, out object? cachedUsers))
            {
                _logger.LogInformation("Users data retrieved from IMemoryCache");
                return Ok(cachedUsers);
            }

            // Fetch users from API
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://jsonplaceholder.typicode.com/users");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch from external API.");

            var json = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<object>(json);

            // Set data in memory cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _memoryCache.Set(cacheKey, users, cacheEntryOptions);
            _logger.LogInformation("Users data fetched from API and cached in memory");

            return Ok(users);
        }
    }
}