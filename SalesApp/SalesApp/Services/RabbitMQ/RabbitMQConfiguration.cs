namespace SalesApp.Services.RabbitMQ
{
    public class RabbitMQConfiguration
    {
        public string HostName { get; set; } 
        public string UserName { get; set; } 
        public string Password { get; set; } 
        public int Port { get; set; } = 5672;
        public string OrderQueueName { get; set; } 
    }
}
