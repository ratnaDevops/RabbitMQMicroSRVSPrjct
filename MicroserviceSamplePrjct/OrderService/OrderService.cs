using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace OrderService
{   
    public class OrderService
    {
        private const string ExchangeName = "order_exchange";
        private const string RoutingKey = "order";

        public void SendOrderMessage(Order order)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
                var serializedOrder = JsonConvert.SerializeObject(order);
                var orderBody = Encoding.UTF8.GetBytes(serializedOrder);
                channel.BasicPublish(exchange: ExchangeName, routingKey: RoutingKey, basicProperties: null, body: orderBody);
                Console.WriteLine($"Sent message: {order}");
            }
        }
    }
}
