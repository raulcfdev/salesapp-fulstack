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
        private IConnection _connection;
        private IModel _channel;
        private bool _isConnected = false;

        public RabbitMQService(IOptions<RabbitMQConfiguration> options)
        {
            _config = options.Value;
            TryConnect();
        }

        private bool TryConnect()
        {
            if (_isConnected) return true;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _config.HostName,
                    UserName = _config.UserName,
                    Password = _config.Password,
                    Port = _config.Port,
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(3)
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: _config.OrderQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _isConnected = true;
                return true;
            }
            catch
            {
                _isConnected = false;
                return false;
            }
        }

        public bool SendMessage<T>(T message, string queueName)
        {
            if (!_isConnected && !TryConnect())
            {
                return false;
            }

            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(message);
                var body = System.Text.Encoding.UTF8.GetBytes(json);

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: null,
                    body: body);

                return true;
            }
            catch
            {
                _isConnected = false;
                return false;
            }
        }

        public void Dispose()
        {
            try
            {
                if (_channel?.IsOpen == true)
                {
                    _channel.Close();
                    _channel.Dispose();
                }

                if (_connection?.IsOpen == true)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            catch
            {
                // Ignora erros no dispose
            }
        }
    }
}
