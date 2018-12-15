using System;
using StackExchange.Redis;

namespace Ex02_StackExchangeRedis
{
    class Program
    {
        static void Main(string[] args)
        {
            var multiplexer = ConnectionMultiplexer.Connect("happybook,password=redisUltraStrongPassword");

            var database = multiplexer.GetDatabase(0);

            database.StringSet("Karel_test", "This is some value");

            Console.WriteLine(database.StringGet("Karel_test"));
        }
    }
}
