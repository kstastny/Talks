using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;

namespace FibListener
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = RabbitMqHelpers.CreateConnection())
            {
                ListenToFibonacciResults(connection);

                Console.WriteLine("Connected to exchange, listening for messages. Press Enter to exit. ");
                Console.ReadLine();
            }

            Console.WriteLine("Exiting program");
        }

        private static void ListenToFibonacciResults(IConnection connection)
        {
            var channel = connection.CreateModel();

            //create queue and bind it to exchange to get the messages
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueName, MessagingConstants.FIBONACCI_BROADCAST_EXCHANGE, "not important for fanout");

            //create consumer that will listen to messages
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received +=
                (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine(message);
                };

            //start listening
            channel.BasicConsume(queueName, autoAck: true, consumer: consumer);
        }
    }
}