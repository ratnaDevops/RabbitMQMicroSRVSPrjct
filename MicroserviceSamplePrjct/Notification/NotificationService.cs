
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Notification
{
  
    public class NotificationService: BackgroundService
    {
        private const string ExchangeName = "order_exchange";
        private const string QueueName = "notification_queue";
        private const string RoutingKey = "shipping";
        private readonly ConnectionFactory _factory;

        public NotificationService()
        {
            _factory = new ConnectionFactory();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
                channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(QueueName, ExchangeName, RoutingKey);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var shippingmessage = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received message: {shippingmessage}");

                    // Process the message here (e.g., send a notification)
                    // Example: SendNotification(message);

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
                Console.WriteLine("NotificationService listening for messages...");
                Console.ReadLine(); // Keep the service running
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }
    }
}
