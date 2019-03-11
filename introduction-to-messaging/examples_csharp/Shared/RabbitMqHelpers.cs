using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Shared
{
    public static class RabbitMqHelpers
    {
        public static IConnection CreateConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = ConnectionConstants.RABBITMQ_HOST,
                Port = ConnectionConstants.RABBITMQ_PORT,
                VirtualHost = ConnectionConstants.RABBITMQ_VIRTUALHOST,
                UserName = ConnectionConstants.RABBITMQ_USERNAME,
                Password = ConnectionConstants.RABBITMQ_PASSWORD
            };

            return factory.CreateConnection();
        }


        public static void DeclareFibonacciExchange(IModel channel)
        {
            channel.ExchangeDeclare(
                MessagingConstants.FIBONACCI_EXCHANGE,
                "direct",
                durable: false,
                autoDelete: false);
        }

        public static void DeclareFibonacciBroadcastExchange(IModel channel)
        {
            channel.ExchangeDeclare(
                MessagingConstants.FIBONACCI_BROADCAST_EXCHANGE,
                "fanout",
                durable: false,
                autoDelete: false);
        }
    }
}