using Confluent.Kafka;
using CosmosDB_Simple_API.Models;
using CosmosDB_Simple_API.Repositories;

namespace CosmosDB_Simple_API.Services
{
    public class KafkaConsumerService : IKafkaConsumerService
    {
        private readonly IConsumer<Null, string> _consumer;
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<KafkaConsumerService> _logger;


        public KafkaConsumerService(ConsumerConfig config, ILogger<KafkaConsumerService> logger, ITaskRepository taskRepository)
        {
            _consumer = new ConsumerBuilder<Null, string>(config).Build();
            _consumer.Subscribe("topic1");
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task Consume(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    _logger.LogInformation($"Consumed message: {consumeResult.Message.Value}");

                    var taskItem = new TaskItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = consumeResult.Message.Value,
                        IsCompleted = false
                    };

                    await _taskRepository.AddTaskAsync(taskItem);
                    _logger.LogInformation($"TaskItem with Id {taskItem.Id} saved to CosmosDB.");
                }
            }
            catch (OperationCanceledException)
            {
                _consumer.Close();
                _logger.LogInformation("Consumer closed due to cancellation.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Kafka consumer.");
            }
        }
    }
}
