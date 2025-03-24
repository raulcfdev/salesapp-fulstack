namespace SalesApp.Services.RabbitMQ
{
    public class RabbitMQConfiguration
    {
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public int Port { get; set; } = 5672;
        public string OrderQueueName { get; set; } = "orders_queue";
    }
}
