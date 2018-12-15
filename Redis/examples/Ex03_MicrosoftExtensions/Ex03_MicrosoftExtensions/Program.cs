using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Newtonsoft.Json;

namespace Ex03_MicrosoftExtensions
{
    class Program
    {
        static void Main(string[] args)
        {
            var cache = new RedisCache(
                new RedisCacheOptions
                {
                    Configuration = "happybook,password=redisUltraStrongPassword",
                    InstanceName = "training:"
                });

            cache.Set("test", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject("This is some object to serialize")), new DistributedCacheEntryOptions());

            var resultJson =
                Encoding.UTF8.GetString(cache.Get("test"));

            Console.WriteLine(resultJson);

            Console.WriteLine(JsonConvert.DeserializeObject<string>(resultJson));
        }
    }
}
