using RabbitMQ.Client;
using System;
using System.Text;

namespace MyRabbitMQArchive.WorkQueues.NewTask
{
    public class NewTask
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.QueueDeclare(queue: "task_queue_with_message_acknowledgements", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var message = GetMessage(args);
                var parsedBody = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: string.Empty, routingKey: "task_queue_with_message_acknowledgements", basicProperties: properties, body: parsedBody);
                
                Console.WriteLine($"Sent body: {message}");
            }
        }

        private static string GetMessage(string[] args)
        {
            return (args.Length > 0) ? String.Join(" ", args) : "Hello World!";
        }
    }
}