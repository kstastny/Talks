using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;

namespace FibAsyncClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = RabbitMqHelpers.CreateConnection())
            {
                RequestFibonaccisAsync(connection);
            }

            Console.WriteLine("Exiting program");
        }

        private static void RequestFibonaccisAsync(IConnection connection)
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

                        //Declare reply queue - anonymous exclusive. Here we will wait for replies
                        var replyQueue = channel.QueueDeclare(durable: false, autoDelete: true, exclusive: true);

                        //create consumer that will listen to reply messages
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received +=
                            (sender, e) =>
                            {
                                var body = e.Body;
                                var message = Encoding.UTF8.GetString(body);

                                Console.WriteLine($"Reply to {e.BasicProperties.CorrelationId}: {message}");
                            };
                        channel.BasicConsume(consumer, replyQueue.QueueName, autoAck: true);

                        var requestedFibonacci = 0;
                        
                        while (token.IsCancellationRequested == false)
                        {
                            var requestProperties = channel.CreateBasicProperties();
                            requestProperties.ReplyTo = replyQueue.QueueName;
                            requestProperties.CorrelationId = Guid.NewGuid().ToString();

                            Console.WriteLine("Requesting Fibonacci number {0}, correlationId {1}", requestedFibonacci, requestProperties.CorrelationId);

                            var body = Encoding.UTF8.GetBytes(requestedFibonacci.ToString());
                            channel.BasicPublish(
                                MessagingConstants.FIBONACCI_EXCHANGE,
                                MessagingConstants.FIBONACCI_CALCULATE_ROUTING_KEY,
                                body: body,
                                mandatory: true,
                                basicProperties:requestProperties
                            );


                            requestedFibonacci = (requestedFibonacci + 1) % 100;

                            Thread.Sleep(2000);
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
