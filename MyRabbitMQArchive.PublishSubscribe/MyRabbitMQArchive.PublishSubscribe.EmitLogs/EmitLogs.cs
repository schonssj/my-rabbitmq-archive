using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace MyRabbitMQArchive.PublishSubscribe.EmitLogs
{
    public class EmitLogs
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                var message = GetMessage(args);
                var parsedBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: parsedBody);

                Console.WriteLine($"Sent body: {message}");
            }
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? String.Join(" ", args) : "Hello World!");
        }
    }
}