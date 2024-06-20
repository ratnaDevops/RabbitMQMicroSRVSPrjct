using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace MicroserviceSamplePrjct.Services.CouponAPI
{
    public class ProducerService
    {
        private readonly RabbitMQ.Client.IModel _channel;
        private readonly string _queueName;

        public ProducerService(IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQConfiguration").Get<RabbitMQConfiguration>();
            _queueName = rabbitMQConfig.QueueName;

            var factory = new ConnectionFactory() { HostName = rabbitMQConfig.Hostname, UserName = rabbitMQConfig.UserName, Password = rabbitMQConfig.Password };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(_queueName, false, false, false, null);
        }

        public void SendMessage(object message)
        {
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
            Console.WriteLine("Sent message: {0}", message);
        }
    }
}
