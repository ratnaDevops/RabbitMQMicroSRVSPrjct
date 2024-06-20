//using Microsoft.Extensions.Hosting;
//using Newtonsoft.Json;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace PaymentsService
//{
//    public class RabbitMQConsumer : BackgroundService
//    {
//        private const string ExchangeName = "order_exchange";
//        private const string QueueName = "payment_queue";
//        private const string RoutingKey = "order";
//        private readonly ConnectionFactory _factory;

//        public RabbitMQConsumer()
//        {
//            _factory = new ConnectionFactory() { HostName = "localhost" };
//        }

       

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            using (var connection = _factory.CreateConnection())
//            using (var channel = connection.CreateModel())
//            {
//                channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
//                channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
//                channel.QueueBind(QueueName, ExchangeName, RoutingKey);

//                var consumer = new EventingBasicConsumer(channel);
//                consumer.Received += async (model, ea) =>
//                {
//                    var body = ea.Body.ToArray();
//                    var message = Encoding.UTF8.GetString(body);
//                    Console.WriteLine($"Received message: {message}");

//                    var payment = JsonConvert.DeserializeObject<Payment>(message);

//                    // Process the payment properties
//                    Console.WriteLine($"PaymentId: {payment.PaymentId}, OrderId: {payment.OrderId}, Amount: {payment.Amount}");

//                    // Send payment properties to RabbitMQ
//                    var paymentMessage = $"PaymentId:{payment.PaymentId}, OrderId: {payment.OrderId}, Amount: {payment.Amount}";
//                    var paymentBody = Encoding.UTF8.GetBytes(paymentMessage);
//                    channel.BasicPublish(exchange: ExchangeName, routingKey: "payment", basicProperties: null, body: paymentBody);
//                    Console.WriteLine($"Sent payment message: {paymentMessage}");


//                };

//                channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
//                Console.WriteLine("PaymentService listening for messages...");

//                await Task.Delay(Timeout.Infinite, stoppingToken);
//            }
//        }
//    }
//}
    

