using In_Memory_Caching.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace In_Memory_Caching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        //Injecting the required services
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductsController(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
        {
            _cache = memoryCache;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            string cacheKey = $"Product_{id}";

            if (_cache.TryGetValue(cacheKey, out Product product))
            {
                return Ok(product); // ✅ Return from cache
            }

            //  Fetch from  API if not in cache
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://jsonplaceholder.typicode.com/posts/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();
            product = JsonSerializer.Deserialize<Product>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // ✅ Store in cache with expiration
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Customize expiration

            _cache.Set(cacheKey, product, cacheEntryOptions);

            return Ok(product);
        }
    }

}

