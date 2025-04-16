using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using w8_d3_classTask.Services.Caching;

namespace w8_d3_classTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        //inject nedded services 
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PostsController> _logger;
        private readonly IRedisCacheService _redisCacheService;


        public PostsController(IHttpClientFactory httpClientFactory, ILogger<PostsController> logger, IRedisCacheService redisCacheService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            _redisCacheService = redisCacheService;
        }



        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            const string cacheKey = "posts";

            var cachedPosts = _redisCacheService.GetData<object>(cacheKey);

            if (cachedPosts is not null)
            {
                _logger.LogInformation(" Data returned from Redis .");
                return Ok(cachedPosts);
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to fetch data from external API.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<object>(json);

            _redisCacheService.SetData(cacheKey, posts);
            _logger.LogInformation("Data fetched from API and cached in Redis.");

            return Ok(posts);
        }

    }
}
