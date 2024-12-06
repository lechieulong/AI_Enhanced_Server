using IService;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _connection;

        public RedisService(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public void SetString(string key, string value)
        {
            var db = _connection.GetDatabase();
            db.StringSet(key, value);
        }

        public string GetString(string key)
        {
            var db = _connection.GetDatabase();
            return db.StringGet(key);
        }
    }

}
