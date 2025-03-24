namespace SalesApp.Services.RabbitMQ
{
    public interface IRabbitMQService
    {
        void SendMessage<T>(T message, string queueName);
    }
}
