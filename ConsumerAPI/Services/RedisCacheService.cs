using System.Text.Json;
using StackExchange.Redis;

namespace ConsumerAPI.Services
{
    public interface ICacheService
    {
        Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task<bool> RemoveAsync(string key);
        Task<bool> KeyExistsAsync(string key);
    }

    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var db = _redis.GetDatabase();
            var cachedValue = await db.StringGetAsync(key);

            if (!cachedValue.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<T>(cachedValue!);
            }

            var value = await factory();
            if (value != null)
            {
                await SetAsync(key, value, expiration ?? _defaultExpiration);
            }

            return value;
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var db = _redis.GetDatabase();
            var serializedValue = JsonSerializer.Serialize(value);
            return await db.StringSetAsync(key, serializedValue, expiration ?? _defaultExpiration);
        }

        public async Task<bool> RemoveAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.KeyDeleteAsync(key);
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync(key);
        }
    }
}
