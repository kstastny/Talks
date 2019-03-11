#r "packages/RabbitMQ.Client/lib/net451/RabbitMQ.Client.dll"

open System
open System.Text
open System.Threading.Tasks

open RabbitMQ.Client.Events
open RabbitMQ.Client

module RabbitMqSettings = 
    let Host = "localhost"
    let Port = 5672

    let VHost = "/"

    let Username = "guest"
    let Password = "guest"


module MessagingSettings = 

    let FibExchange = "fibonacci"
    let FibCalculateRoutingKey = "fibonacci.calculate"
    let FibBroadcastExchange = "fibonacci.broadcast"



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



/// Request-Reply example. Note that normally we would also reuse the Channel but for simplicity we always create a new one
let requestFibAsync (conn: IConnection) (fib: int) : Task<string> =
    let correlationId = Guid.NewGuid() |> string
    let taskCompletionSource = TaskCompletionSource<string>()

    let channel = conn.CreateModel ()
    channel.BasicReturn |> Event.add (fun e -> 
        match e.ReplyCode with
        | 312us -> printfn "Message could not be delivered to consumer"
                   taskCompletionSource.SetResult("ERROR delivering the message") //should be exception obviously, or ROP Error
        | _ -> printfn "Basic return - %i: %s" e.ReplyCode e.ReplyText
        )

    //create consumer that will receive the message
    let consumer = EventingBasicConsumer(channel)
    consumer.Received
    |> Event.add (fun e -> 
        e.Body |> Encoding.UTF8.GetString
        //|> (fun x -> printfn "Message for correlationId %s: %s" e.BasicProperties.CorrelationId x; x)
        |> taskCompletionSource.SetResult
    )

    //start listening on direct reply-to queue, see https://www.rabbitmq.com/direct-reply-to.html
    channel.BasicConsume("amq.rabbitmq.reply-to", autoAck=true, consumer=consumer) |> ignore

    //send message
    let requestProperties = channel.CreateBasicProperties();
    requestProperties.ReplyTo <- "amq.rabbitmq.reply-to";
    requestProperties.CorrelationId <- correlationId

    let body = fib |> string |> Encoding.UTF8.GetBytes
    channel.BasicPublish(
        MessagingSettings.FibExchange,
        MessagingSettings.FibCalculateRoutingKey,
        body = body,
        mandatory = true,
        basicProperties = requestProperties
    )    


    //simple timeout, using Polly would be better
    Task.WhenAny(Task.Delay(5000), taskCompletionSource.Task)
        .ContinueWith(fun (t: Task) -> 
            if not (System.Object.ReferenceEquals(t, taskCompletionSource.Task)) then 
                taskCompletionSource.SetResult("Timeout")
        ) |> ignore

    taskCompletionSource.Task




//----------- PLAYGROUND BELOW    
let connection = createConnection ()
let channel = createChannel connection
let fib = requestFibCommand channel
createFibListener channel |> ignore

fib 15

let fibAsync fib =
    let t = requestFibAsync connection fib
    t.ContinueWith(fun (x: Task<string>) -> printfn "RESULT = %s" x.Result) |> ignore
    
    
fibAsync 1



channel.Close ()
connection.Close ()