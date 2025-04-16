using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using w8_d3_classTask.Services.Caching;

namespace w8_d3_classTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //inject nedded services 
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UsersController> _logger;
        private readonly IRedisCacheService _redisCacheService;



        public UsersController(IHttpClientFactory httpClientFactory, IRedisCacheService redisCacheService, ILogger<UsersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            const string CacheKey = "users";
            var cachedUsers = _redisCacheService.GetData<object>(CacheKey);

            if (cachedUsers is not null)
            {
                _logger.LogInformation("✅ Users data returned from Redis cache.");
                return Ok(cachedUsers);
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://jsonplaceholder.typicode.com/users");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "❌ Failed to fetch data from external API.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<object>(json);

            _redisCacheService.SetData(CacheKey, users);
            _logger.LogInformation(" Users data fetched from API and cached in Redis.");

            return Ok(users);
        }
    }
}
    

