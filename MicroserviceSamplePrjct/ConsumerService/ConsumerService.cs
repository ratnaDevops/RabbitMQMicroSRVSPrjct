using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ConsumerService
{
    public class ConsumerService
    {
        private readonly IModel _channel;
        private readonly string _queueName;
        private string _lastReceivedMessage;

        public ConsumerService(IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQConfiguration").Get<RabbitMQConfiguration>();
            _queueName = rabbitMQConfig.QueueName;

            var factory = new ConnectionFactory() { HostName = rabbitMQConfig.Hostname};
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            ConsumeMessages();
        }

        public void ConsumeMessages()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                if (!body.IsEmpty)
                {
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine("Received message: {0}", message);
                    _lastReceivedMessage = message;
                }
                else
                {
                    Console.WriteLine("Received empty message");
                }
            };
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        public string GetLastReceivedMessage()
        {
            return _lastReceivedMessage;
        }

        internal async Task ReceiveMessage()
        {
            throw new NotImplementedException();
        }
    }
}