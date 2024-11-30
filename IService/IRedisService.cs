using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IRedisService
    {
        void SetString(string key, string value);
        string GetString(string key);
    }
}
