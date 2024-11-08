using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace ProducerAPI.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(ProducerConfig config, ILogger<KafkaProducerService> logger)
        {
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _logger = logger;
            _logger.LogInformation("Kafka producer initialized.");
        }

        public async Task ProduceAsync(string topic, string message)
        {
            try
            {
                _logger.LogInformation($"Producing message to topic '{topic}': {message}");

                var deliveryResult = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });

                _logger.LogInformation($"Message produced to topic '{deliveryResult.TopicPartitionOffset}': {message}");
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex, $"Error producing message to topic '{topic}': {ex.Error.Reason}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while producing the message.");
            }
        }
    }
}
