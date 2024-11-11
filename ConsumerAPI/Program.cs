using Confluent.Kafka;
using ConsumerAPI.Services;
using CosmosDB_Simple_API.Repositories;
using CosmosDB_Simple_API.Services;
using Microsoft.Azure.Cosmos;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Redis 
var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString");
var redis = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "MyRedisCache";
});

// Cosmos
var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb");
string cosmosEndpoint = cosmosDbConfig["Endpoint"];
string cosmosKey = cosmosDbConfig["Key"];
string databaseName = cosmosDbConfig["DatabaseName"];
string containerName = cosmosDbConfig["ContainerName"];

Console.WriteLine($"CosmosDB Endpoint: {cosmosEndpoint}");

CosmosClientOptions options = new()
{
    HttpClientFactory = () => new HttpClient(new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }),
    ConnectionMode = ConnectionMode.Gateway,
};

CosmosClient cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey, options);

Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
await database.CreateContainerIfNotExistsAsync(containerName, "/id");

// Kafka
var consumerConfig = new ConsumerConfig
{
    BootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers"),
    GroupId = builder.Configuration.GetValue<string>("Kafka:GroupId"),
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = true
};


builder.Services.AddSingleton(consumerConfig);
builder.Services.AddSingleton<ITaskRepository>(new TaskRepository(cosmosClient, databaseName, containerName));
builder.Services.AddSingleton<ITaskCacheService, TaskCacheService>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();
builder.Services.AddHostedService<KafkaConsumerHostedService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
