using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OrderService;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentsService
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
                    var messageBodyBytes = ea.Body.ToArray();
                    var ordermessage = Encoding.UTF8.GetString(messageBodyBytes);
                    Console.WriteLine($"Received message: {ordermessage}");

                    var payment = JsonConvert.DeserializeObject<Payment>(ordermessage);
                    //var order = JsonConvert.DeserializeObject<Order>(ordermessage);

                    ProcessPaymentAndSendPayment(payment, channel);
                    //SendToRabbitMQ(payment, channel);

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
                Console.WriteLine("PaymentService listening for messages...");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }

        private void ProcessPaymentAndSendPayment(Payment payment, IModel channel)
        {
            var processedPayment = CreatePayment(payment);
            var serializedPayment = JsonConvert.SerializeObject(processedPayment);
            var paymentBody = Encoding.UTF8.GetBytes(serializedPayment);
            channel.BasicPublish(exchange: ExchangeName, routingKey: "payment", basicProperties: null, body: paymentBody);
            Console.WriteLine($"Sent payment message: {serializedPayment}");
        }

        private Payment CreatePayment(Payment payments)
        {
            var payment = new Payment
            {
                OrderId = payments.OrderId,
                PaymentId=1,
                Amount= payments.Amount,

            };

            return payment;
        }

        public void SendToRabbitMQ(Payment payment, IModel channel)
        {
            var processedPayment = ProcessPaymentProperties(payment);
            var serializedPayment = JsonConvert.SerializeObject(processedPayment);
            var paymentBody = Encoding.UTF8.GetBytes(serializedPayment);
            channel.BasicPublish(exchange: ExchangeName, routingKey: "payment", basicProperties: null, body: paymentBody);
            Console.WriteLine($"Sent payment message: {serializedPayment}");
        }

        public Dictionary<string, object> ProcessPaymentProperties(Payment payment)
        {                    
            var paymentMessage = $"PaymentId:{payment.PaymentId}, OrderId: {payment.OrderId}, Amount: {payment.Amount}";
         
            var paymentMessageparts = paymentMessage.Split(new char[] { ',', ':' }, StringSplitOptions.RemoveEmptyEntries);
          
            var paymentMessagedict = new Dictionary<string, object>();
            for (int i = 0; i < paymentMessageparts.Length; i += 2)
            {
                var key = paymentMessageparts[i].Trim();
                var value = paymentMessageparts[i + 1].Trim();
                paymentMessagedict.Add(key, value);
            }

            return paymentMessagedict;
        }

    }
}


