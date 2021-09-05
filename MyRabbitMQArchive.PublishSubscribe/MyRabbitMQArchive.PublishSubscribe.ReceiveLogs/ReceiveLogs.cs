using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace MyRabbitMQArchive.PublishSubscribe.ReceiveLogs
{
    public class ReceiveLogs
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");

                var eventingBasicConsumer = new EventingBasicConsumer(channel);
                eventingBasicConsumer.Received += (model, ea) =>
                {
                    var parsedBody = ea.Body.ToArray();
                    var receivedMessage = Encoding.UTF8.GetString(parsedBody);
                    Console.WriteLine($"Received message: {receivedMessage}");
                };
                channel.BasicConsume(queue: queueName, autoAck: false, consumer: eventingBasicConsumer);

                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }
        }
    }
}