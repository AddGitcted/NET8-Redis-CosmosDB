using CosmosDB_Simple_API.Services;

public class KafkaConsumerHostedService : BackgroundService
{
    private readonly IKafkaConsumerService _consumerService;
    private readonly ILogger<KafkaConsumerHostedService> _logger;

    public KafkaConsumerHostedService(IKafkaConsumerService consumerService, ILogger<KafkaConsumerHostedService> logger)
    {
        _consumerService = consumerService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KafkaConsumerHostedService is starting.");
        await _consumerService.Consume(stoppingToken);
    }
}
