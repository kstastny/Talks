using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class MessagingConstants
    {
        public const string FIBONACCI_EXCHANGE = "fibonacci";
        public const string FIBONACCI_QUEUE_CALCULATE = "fibonacci.calculate";
        public const string FIBONACCI_CALCULATE_ROUTING_KEY = "fibonacci.calculate";
        public const string FIBONACCI_BROADCAST_EXCHANGE = "fibonacci.broadcast";
    }
}