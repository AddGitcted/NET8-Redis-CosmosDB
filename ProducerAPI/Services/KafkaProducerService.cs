using Confluent.Kafka;

namespace ProducerAPI.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(ProducerConfig config)
        {
            _producer = new ProducerBuilder<Null,  string>(config).Build();
        }

        public async Task ProduceAsync(string topic, string message)
        {
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            Console.WriteLine($"Produced message: {message}");
        }
    }
}
