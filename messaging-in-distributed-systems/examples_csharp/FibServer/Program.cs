using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using IConnection = RabbitMQ.Client.IConnection;

namespace FibServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = RabbitMqHelpers.CreateConnection())
            {
                ListenToFibonacciRequests(connection);
            }
        }

        private static void ListenToFibonacciRequests(IConnection connection)
        {
            using (var channel = connection.CreateModel())
            {
                channel.CallbackException += Channel_CallbackException;
                RabbitMqHelpers.DeclareFibonacciExchange(channel);
                RabbitMqHelpers.DeclareFibonacciBroadcastExchange(channel);


                //declare the queue - this will create it if it does not exist. durable: queue is not persisted, is lost after server restart. 
                //AutoDelete && exclusive is false - queue will stay on server even after this client disconnects
                channel.QueueDeclare(MessagingConstants.FIBONACCI_QUEUE_CALCULATE, durable: false, autoDelete: false,
                    exclusive: false);

                //bind to fibonacci queue - the name is given because there will be only one worker for each task
                channel.QueueBind(MessagingConstants.FIBONACCI_QUEUE_CALCULATE, MessagingConstants.FIBONACCI_EXCHANGE,
                    MessagingConstants.FIBONACCI_CALCULATE_ROUTING_KEY);

                var lChannel = channel;

                //create consumer that will listen to messages
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received +=
                    (sender, e) =>
                    {
                        var body = e.Body;
                        var message = Encoding.UTF8.GetString(body);
                        //fibonacci message is just a number for which we should calculate fibonacci number
                        var requestedFibonacci = int.Parse(message);

                        Console.WriteLine("Computing Fibonacci {0}", requestedFibonacci);
                        var stopWatch = new Stopwatch();
                        stopWatch.Start();
                        var fibNumber = Fibonacci(requestedFibonacci);
                        stopWatch.Stop();

                        //send the result to broadcast
                        var outMessage =
                            $"{Environment.MachineName}: Fibonacci {requestedFibonacci} is {fibNumber}, calculation took {stopWatch.Elapsed.ToString()}";
                        Console.WriteLine(outMessage);

                        var receivedMessageProperties = e.BasicProperties;
                        if (!string.IsNullOrWhiteSpace(receivedMessageProperties.ReplyTo))
                        {
                            //RPC call, just reply to whoever wanted the calculation
                            var replyProperties = lChannel.CreateBasicProperties();
                            replyProperties.CorrelationId = receivedMessageProperties.CorrelationId;
                            Console.WriteLine(
                                $"Sending the result to reply queue {receivedMessageProperties.ReplyTo}, correlationId {receivedMessageProperties.CorrelationId}");

                            lChannel.BasicPublish(
                                exchange: "",
                                routingKey: receivedMessageProperties.ReplyTo,
                                body: Encoding.UTF8.GetBytes(outMessage),
                                basicProperties: replyProperties);
                        }
                        else
                        {
                            //broadcast the result to everyone
                            lChannel.BasicPublish(
                                MessagingConstants.FIBONACCI_BROADCAST_EXCHANGE,
                                "",
                                body: Encoding.UTF8.GetBytes(outMessage));
                        }


                        //acknowledge the job so it is not sent to other workers
                        lChannel.BasicAck(e.DeliveryTag, false);
                    };

                //set QOS to only get so many messages at once
                channel.BasicQos(0, 3, false);

                //start receiving calculation requests. NOTE that we're setting autoAck to false, that means that we have to acknowledge processed messages
                channel.BasicConsume(
                    consumer: consumer,
                    queue: MessagingConstants.FIBONACCI_QUEUE_CALCULATE,
                    autoAck: false);


                Console.WriteLine("Listening to fibonacci requests. Press Enter to exit.");

                Console.ReadLine();
            }
        }

        private static void Channel_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            Console.WriteLine("Channel_CallbackException " + e.Exception);
        }


        private static double Fibonacci(int fib)
        {
            if (fib < 0)
                return 0;

            var previousFibonacci = 0d;
            var currentFibonacci = 1d;

            for (var remainingSteps = fib; remainingSteps > 0; remainingSteps--)
            {
                //simulate some hard computation
                Thread.Sleep(100);
                var newFibonacci = previousFibonacci + currentFibonacci;
                previousFibonacci = currentFibonacci;
                currentFibonacci = newFibonacci;
            }

            return currentFibonacci;
        }
    }
}