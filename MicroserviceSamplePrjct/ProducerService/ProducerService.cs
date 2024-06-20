using RabbitMQ.Client;
using System.Text;

namespace ProducerService
{
    public class ProducerService
    {
        private readonly IModel _channel;
        private readonly string _queueName;

        public ProducerService(IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQConfiguration").Get<RabbitMQConfiguration>();
            _queueName = rabbitMQConfig.QueueName;

            var factory = new ConnectionFactory() { HostName = rabbitMQConfig.Hostname,UserName=rabbitMQConfig.UserName,Password=rabbitMQConfig.Password};
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(_queueName,false,false,false,null);
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
            Console.WriteLine("Sent message: {0}", message);
        }
    }
}