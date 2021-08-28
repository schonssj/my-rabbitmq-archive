using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace MyRabbitMQArchive.HelloWorld
{
    public class MessageReceiver
    {
        public static void Main()
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var eventingBasicConsumer = new EventingBasicConsumer(channel);

                eventingBasicConsumer.Received += (model, ea) =>
                {
                    var parsedBody = ea.Body.ToArray();
                    var receivedMessage = Encoding.UTF8.GetString(parsedBody);
                    Console.WriteLine($"Received message: {receivedMessage}");
                };

                channel.BasicConsume(queue: "hello", autoAck: true, consumer: eventingBasicConsumer);
                Console.ReadLine();
            }
        }
    }
}