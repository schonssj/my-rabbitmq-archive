using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace MyRabbitMQArchive.Routing.EmitLogDirect
{
    public class EmitLogDirect
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");

                var severity = args.Length > 0 ? args[0] : "info";
                var message = args.Length > 1 ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
                var parsedBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "direct_logs", routingKey: severity, basicProperties: null, body: parsedBody);

                Console.WriteLine("Sent '{0}':'{1}'", severity, message);
            }
        }
    }
}