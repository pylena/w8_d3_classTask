namespace w8_d3_classTask.Services.Caching
{
    public interface IRedisCacheService
    {
        // extract data from cache
        T? GetData<T>(string key);
        // set data in cache
        void SetData<T>(string key, T data);
    }
}
