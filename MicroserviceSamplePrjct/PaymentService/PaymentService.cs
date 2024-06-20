
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PaymentService
{
    public class PaymentService : BackgroundService
    {
        private const string ExchangeName = "order_exchange";
        private const string QueueName = "payment_queue";
        private const string RoutingKey = "order";
        private readonly ConnectionFactory _factory;

        public PaymentService()
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
                consumer.Received += async (model, ea) =>
                {
                    var orderBody = ea.Body.ToArray();
                    var order = Encoding.UTF8.GetString(orderBody);
                    Console.WriteLine($"Received message: {order}");

                    var payment = JsonConvert.DeserializeObject<Payment>(order);                 
                    SendPayment(payment, channel);                   

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);             
              
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }

        private void SendPayment(Payment payments, IModel channel)
        {
            var payment = CreatePayment(payments);
            var serializedPayment = JsonConvert.SerializeObject(payment);
            var paymentBody = Encoding.UTF8.GetBytes(serializedPayment);
            channel.BasicPublish(exchange: ExchangeName, routingKey: "payment", basicProperties: null, body: paymentBody);
            Console.WriteLine($"Sent payment message: {serializedPayment}");
        }

        private Payment CreatePayment(Payment payments)
        {
            var payment = new Payment
            {
                OrderId = payments.OrderId,
                PaymentId = 1,
                Amount = payments.Amount,

            };

            return payment;
        }
    }
}
