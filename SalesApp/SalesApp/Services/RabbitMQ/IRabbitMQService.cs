namespace SalesApp.Services.RabbitMQ
{
    public interface IRabbitMQService
    {
        bool SendMessage<T>(T message, string queueName);
    }
}
