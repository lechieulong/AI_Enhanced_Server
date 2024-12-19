using IService;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RedisService : IDisposable
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisService(string connectionString)
        {
            // Connect to Azure Redis Cache
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase();
        }

        // Add or update a key-value pair
        public async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            return await _db.StringSetAsync(key, value, expiry);
        }

        // Retrieve the value of a key
        public async Task<string?> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        // Delete a key
        public async Task<bool> DeleteAsync(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
