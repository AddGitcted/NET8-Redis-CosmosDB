using Confluent.Kafka;
using CosmosDB_Simple_API.Models;
using CosmosDB_Simple_API.Repositories;
using Microsoft.Extensions.Logging;

namespace CosmosDB_Simple_API.Services
{
    public class KafkaConsumerService : IKafkaConsumerService
    {
        private readonly IConsumer<Null, string> _consumer;
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(ConsumerConfig config, ILogger<KafkaConsumerService> logger, ITaskRepository taskRepository)
        {
            _logger = logger;
            _taskRepository = taskRepository;

            try
            {
                _logger.LogInformation("Initializing Kafka consumer...");
                _consumer = new ConsumerBuilder<Null, string>(config).Build();
                _logger.LogInformation("Kafka consumer initialized.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Kafka consumer.");
                throw;
            }
        }

        public async Task Consume(CancellationToken cancellationToken)
        {
            _consumer.Subscribe("topic1");
            _logger.LogInformation("Subscribed to topic: topic1");

            try
            {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            _logger.LogInformation("Waiting for messages...");
                            var consumeResult = _consumer.Consume(cancellationToken);
                            if (consumeResult != null)
                            {
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
                        catch (ConsumeException ex)
                        {
                            _logger.LogError($"Consume error: {ex.Error.Reason}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An error occurred in the Kafka consumer loop.");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Kafka consumer closed due to cancellation.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the Kafka consumer.");
                }
                finally
                {
                    _consumer.Close();
                    _logger.LogInformation("Kafka consumer closed.");
                }
            }
        }
    }