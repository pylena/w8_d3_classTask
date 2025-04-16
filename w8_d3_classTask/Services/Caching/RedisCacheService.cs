using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace w8_d3_classTask.Services.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public T? GetData<T>(string key)
        {
            // get data based on key
            var data = _cache.GetString(key);

            if (data is null)
            {
                return default;
            }

          return JsonSerializer.Deserialize<T>(data);
        }

        public void SetData<T>(string key, T data)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            // set data in cache
            _cache.SetString(key, JsonSerializer.Serialize(data), options);
            throw new NotImplementedException();
        }
    }
}
