using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorExamples
{
    static class FakeLogs
    {
        public static void LogActivity(string message)
        {
            Console.WriteLine("***  " + message);
        }

        public static void AuditLog(string message)
        {
            Console.WriteLine("AUDIT LOG, User " + Environment.UserDomainName + ": " + message);
        }
    }
}
