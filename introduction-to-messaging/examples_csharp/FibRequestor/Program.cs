using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Shared;

namespace FibRequestor
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = RabbitMqHelpers.CreateConnection())
            {
                ConnectToWorkerQueueAndRequestFibonaccis(connection);
            }

            Console.WriteLine("Exiting program");
        }

        private static void ConnectToWorkerQueueAndRequestFibonaccis(IConnection connection)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            var task = new TaskFactory().StartNew(
                () =>
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.BasicReturn += Channel_BasicReturn;
                        RabbitMqHelpers.DeclareFibonacciExchange(channel);

                        var requestedFibonacci = 0;
                        while (token.IsCancellationRequested == false)
                        {
                            Console.WriteLine("Requesting Fibonacci number {0}", requestedFibonacci);

                            var body = Encoding.UTF8.GetBytes(requestedFibonacci.ToString());
                            channel.BasicPublish(
                                MessagingConstants.FIBONACCI_EXCHANGE,
                                MessagingConstants.FIBONACCI_CALCULATE_ROUTING_KEY,
                                body: body,
                                mandatory:true
                            );
                            

                            requestedFibonacci = (requestedFibonacci + 1) % 100;

                            Thread.Sleep(1000);
                        }

                        Console.WriteLine("Fibonacci requesting canceled.");
                    }
                }, cancellationTokenSource.Token);

            Console.ReadLine();

            cancellationTokenSource.Cancel();
            task.Wait(2000);
        }

        private static void Channel_BasicReturn(object sender, RabbitMQ.Client.Events.BasicReturnEventArgs e)
        {
            Console.WriteLine($"Basic return - {e.ReplyCode}: {e.ReplyText}");
        }
    }
}
