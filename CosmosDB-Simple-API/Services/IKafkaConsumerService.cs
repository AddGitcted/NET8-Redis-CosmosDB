namespace CosmosDB_Simple_API.Services
{
    public interface IKafkaConsumerService
    {
        Task Consume(CancellationToken cancellationToken);
    }
}
