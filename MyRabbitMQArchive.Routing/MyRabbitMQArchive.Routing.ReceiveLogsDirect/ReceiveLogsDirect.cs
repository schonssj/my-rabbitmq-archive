using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace MyRabbitMQArchive.Routing.ReceiveLogsDirect
{
    public class ReceiveLogsDirect
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");
                var queueName = channel.QueueDeclare().QueueName;
                
                if(args.Length < 1)
                {
                    Console.Error.WriteLine("Usage: dotnet run [info] [warning] [error]", Environment.GetCommandLineArgs()[0]);
                    Console.WriteLine("Press enter to exit.");
                    Console.ReadLine();
                    Environment.ExitCode = 1;
                    return;
                }

                foreach (var severity in args)
                    channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: severity);

                Console.WriteLine("Waiting for messages.");

                var eventingBasicConsumer = new EventingBasicConsumer(channel);
                eventingBasicConsumer.Received += (model, ea) =>
                {
                    var parsedBody = ea.Body.ToArray();
                    var receivedMessage = Encoding.UTF8.GetString(parsedBody);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine("Received '{0}':'{1}'", routingKey, receivedMessage);
                };
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: eventingBasicConsumer);

                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }
        }
    }
}