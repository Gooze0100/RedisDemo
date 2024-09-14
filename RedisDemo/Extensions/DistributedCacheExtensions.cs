using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RedisDemo.Extensions
{
    public static class DistributedCacheExtensions
    {
        // <T> is a model for function
        public static async Task SetRecordAsync<T>(this IDistributedCache cache, 
            string recordId, 
            T data, 
            TimeSpan? absoluteExpireTime = null, 
            TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            // with this option in cache value will be saved just for short period of time, make short time
            // it will be deleted by redis
            // if null use right value
            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
            // SlidingExpiration if you are not accessing date for some time it will be deleted event if above is set to 1day or more
            // based upon use
            options.SlidingExpiration = unusedExpireTime;

            var jsonData = JsonSerializer.Serialize(data);
            // passing: key, value and options we have
            await cache.SetStringAsync(recordId, jsonData, options);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recodrdId)
        {
            var jsonData = await cache.GetStringAsync(recodrdId);

            // nice you can use is and no equal
            if (jsonData is null)
            {
                // it will return defaul value that was given, this is generic way of doing it
                // we can give int and we cannot return null because int cannot be null so we just return same value
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
