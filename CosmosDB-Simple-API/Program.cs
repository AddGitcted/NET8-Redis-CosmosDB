using CosmosDB_Simple_API.Repositories;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb");
string account = cosmosDbConfig["Account"];
string key = cosmosDbConfig["Key"];
string databaseName = cosmosDbConfig["DatabaseName"];
string containerName = cosmosDbConfig["ContainerName"];

CosmosClient cosmosClient = new CosmosClient(account, key);

Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
await database.CreateContainerIfNotExistsAsync(containerName, "/id");

builder.Services.AddSingleton(cosmosClient);
builder.Services.AddSingleton<ITaskRepository>(new TaskRepository(cosmosClient, databaseName, containerName));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
