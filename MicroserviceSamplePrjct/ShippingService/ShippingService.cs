using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace ShippingService
{
    public class ShippingService : BackgroundService
    {

        private const string ExchangeName = "order_exchange";
        private const string QueueName = "shipping_queue";
        private const string RoutingKey = "payment";

        private readonly ConnectionFactory _factory;

        public ShippingService()
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };
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
                    var paymentBody = ea.Body.ToArray();
                    var payment = Encoding.UTF8.GetString(paymentBody);
                    Console.WriteLine($"Received message from PaymentService: {payment}");

                    // Deserialize the message into Payment object
                    var shipment = JsonConvert.DeserializeObject<Shipment>(payment);

                    SendShipment(shipment, channel);                   

                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

                Console.WriteLine("ShippingService listening for messages...");
                    Console.ReadLine(); // Wait for user input to exit
            }
        }

        private void SendShipment(Shipment shipments, IModel channel)
        {
            var shipment = CreateShipment(shipments);           
           
            var serializedShipment = JsonConvert.SerializeObject(shipment);
            var shipmentBody = Encoding.UTF8.GetBytes(serializedShipment);
            channel.BasicPublish(exchange: ExchangeName, routingKey: "shipment", basicProperties: null, body: shipmentBody);
            Console.WriteLine($"Sent shipment message: {serializedShipment}");
        }

        private Shipment CreateShipment(Shipment shipments)
        {
           
            var shipment = new Shipment
            {
                OrderId = shipments.OrderId,
                TrackingId = GenerateTrackingId() 
            };

            return shipment;
        }
        private string GenerateTrackingId()
        {          
            return Guid.NewGuid().ToString();
        }
    }
}
