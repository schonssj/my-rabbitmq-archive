using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace MyRabbitMQArchive.WorkQueues.Worker
{
    public class Worker
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.QueueDeclare(queue: "task_queue_with_message_acknowledgements", durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                
                var eventingBasicConsumer = new EventingBasicConsumer(channel);
                eventingBasicConsumer.Received += (model, ea) =>
                {
                    var parsedBody = ea.Body.ToArray();
                    var receivedMessage = Encoding.UTF8.GetString(parsedBody);
                    Console.WriteLine($"Received message: {receivedMessage}");

                    int dots = receivedMessage.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    Console.WriteLine("Done!");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "task_queue_with_message_acknowledgements", autoAck: false, consumer: eventingBasicConsumer);

                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }
        }
    }
}