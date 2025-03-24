using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace SalesApp.Services.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly RabbitMQConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(IOptions<RabbitMQConfiguration> options)
        {
            _config = options.Value;
            var factory = new ConnectionFactory
            {
                HostName = _config.HostName,
                UserName = _config.UserName,
                Password = _config.Password,
                Port = _config.Port
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: _config.OrderQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public void SendMessage<T>(T message, string queueName)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queueName,
                basicProperties: properties,
                body: body);
        }

        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
            }
            if (_connection.IsOpen)
            {
                _connection.Close();
            }
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
