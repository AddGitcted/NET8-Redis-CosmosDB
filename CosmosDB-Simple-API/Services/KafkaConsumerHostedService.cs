namespace CosmosDB_Simple_API.Services
{
    public class KafkaConsumerHostedService : BackgroundService
    {
        private readonly IKafkaConsumerService _consumerService;

        public KafkaConsumerHostedService(IKafkaConsumerService consumerService)
        {
            _consumerService = consumerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumerService.Consume(stoppingToken);
        }
    }
}
