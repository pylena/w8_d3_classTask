using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace w8_d3_classTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //inject nedded services 
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _cache;
        private readonly ILogger<UsersController> _logger;


        public UsersController(IHttpClientFactory httpClientFactory, IDistributedCache cache, ILogger<UsersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            string cacheKey = "users";
            string? cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Users data from Redis Cache");
                var cachedUsers = JsonSerializer.Deserialize<object>(cachedData);
                return Ok(cachedUsers);
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://jsonplaceholder.typicode.com/users");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to fetch from external API.");
            }

            var json = await response.Content.ReadAsStringAsync();
            await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            _logger.LogInformation("Users data fetched from API and cached in Redis");
            var users = JsonSerializer.Deserialize<object>(json);
            return Ok(users);
        }
    }
}
    

