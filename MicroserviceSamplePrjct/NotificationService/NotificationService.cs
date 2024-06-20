using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace NotificationService
{
    public class NotificationService : BackgroundService
    {
        private const string ExchangeName = "order_exchange";
        private const string QueueName = "notification_queue";
        private const string RoutingKey = "shipment";
        private readonly ConnectionFactory _factory;

        public NotificationService()
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
                    var body = ea.Body.ToArray();
                    var shipmentmessage = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received message: {shipmentmessage}");

                    
                    var notification = JsonConvert.DeserializeObject<Notification>(shipmentmessage);

                    // Send the notification details through email
                    //SendEmail(notification);                 
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                
                channel.BasicConsume(QueueName, false, consumer);                
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }

        private void SendEmail(Notification notification)
        {
            using (var client = new SmtpClient("smtp.yourmailserver.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential("your-email@example.com", "your-email-password");
                client.EnableSsl = true;

               
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@example.com"),
                    Subject = "Notification Details",
                    Body = $"NotificationId: {notification.NotificationId}\nOrderId: {notification.OrderId}\nMessage: {notification.Message}\nRecipientEmail: {notification.RecipientEmail}"
                };

                mailMessage.To.Add("your-email@example.com");

                client.Send(mailMessage);
            }
        }
    }
}
