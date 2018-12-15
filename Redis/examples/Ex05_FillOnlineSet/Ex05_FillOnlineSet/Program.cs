using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Ex05_FillOnlineSet
{
    class Program
    {
        static void Main(string[] args)
        {
            var multiplexer = ConnectionMultiplexer.Connect("happybook,password=redisUltraStrongPassword");

            var database = multiplexer.GetDatabase(0);

            var onlineObjects =
                Enumerable.Range(1, 1000)
                    .Where(i => i % 2 != 0 && i % 3 != 0)
                    .Select(i => (RedisValue)i)
                    .ToArray();

            database.SetAdd("online:objects", onlineObjects);

            Console.WriteLine("Set length = " + database.SetLength("online:objects"));
        }
    }
}
