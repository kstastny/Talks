#r "packages/RabbitMQ.Client/lib/net451/RabbitMQ.Client.dll"
open RabbitMQ.Client.Events
open System.Threading
open RabbitMQ.Client

module RabbitMqSettings = 
    let Host = "localhost"
    let Port = 5672

    let VHost = "/"

    let Username = "guest"
    let Password = "guest"


module MessagingSettings = 

    let FibExchange = "fibonacci"
    let FibQueueCalculate = "fibonacci.calculate"
    let FibCalculateRoutingKey = "fibonacci.calculate"
    let FibBroadcastExchange = "fibonacci.broadcast"




open System.Text

open RabbitMQ.Client
open RabbitMQ.Client.Events

let createConnection = 
    let factory = new ConnectionFactory()
    factory.HostName <- RabbitMqSettings.Host
    factory.Port <-  RabbitMqSettings.Port
    factory.VirtualHost <-  RabbitMqSettings.VHost
    factory.UserName <-  RabbitMqSettings.Username
    factory.Password <-  RabbitMqSettings.Password
    
    fun () -> factory.CreateConnection ()

let createChannel (conn: IConnection) = 
    let channel = conn.CreateModel ()
    channel.BasicReturn |> Event.add (fun e -> printfn "Basic return - %i: %s" e.ReplyCode e.ReplyText)

    channel


/// Command - requests that fibonacci number is calculated. Does not return value
let requestFibCommand (channel: IModel) (fib: int) =

    let body = fib |> string |> Encoding.UTF8.GetBytes
    channel.BasicPublish(
        MessagingSettings.FibExchange,
        MessagingSettings.FibCalculateRoutingKey,
        body = body,
        mandatory = true
    )


// Event handler - listen to broadcasted Fibonacci results
let createFibListener (channel: IModel) =

    let queue = channel.QueueDeclare()
    channel.QueueBind(queue.QueueName, MessagingSettings.FibBroadcastExchange, "ignored")

    //create consumer that will listen to messages
    let consumer = EventingBasicConsumer(channel)
    consumer.Received
    |> Event.add (fun e -> 
        e.Body |> Encoding.UTF8.GetString |> (printfn "Consumer %s received message: %s" e.ConsumerTag)
    )

    //start listening
    channel.BasicConsume(queue.QueueName, autoAck=true, consumer=consumer) |> printfn "Listening, consumer tag: %s"

    consumer



//TODO request-reply (simple or "full" implementation?)




//----------- PLAYGROUND BELOW    
let connection = createConnection ()
let channel = createChannel connection
let fib = requestFibCommand channel
createFibListener channel |> ignore

fib 15


channel.Close ()
connection.Close ()