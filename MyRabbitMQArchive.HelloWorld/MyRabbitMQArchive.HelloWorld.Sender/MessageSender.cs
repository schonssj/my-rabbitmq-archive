using RabbitMQ.Client;
using System;
using System.Text;

namespace MyRabbitMQArchive.HelloWorld
{
    public class MessageSender
    {
        public static void Main()
        {
            var message = "Hello World!";
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var parsedBody = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty, routingKey: "hello", basicProperties: null, body: parsedBody);

            Console.WriteLine($"Sent body: {message}");
            Console.ReadLine();
        }
    }
}