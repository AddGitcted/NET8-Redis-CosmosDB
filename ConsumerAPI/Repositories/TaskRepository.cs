﻿using CosmosDB_Simple_API.Models;
using Microsoft.Azure.Cosmos;

namespace CosmosDB_Simple_API.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly Container _container;

        public TaskRepository(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            await _container.CreateItemAsync(task, new PartitionKey(task.Id));
        }

        public async Task<bool> DeleteTaskAsync(string id)
        {
            var container = _container;
            try
            {
                await container.DeleteItemAsync<TaskItem>(id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task<TaskItem?> GetTaskAsync(string id)
        {
            try
            {
                ItemResponse<TaskItem> response = await _container.ReadItemAsync<TaskItem>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<TaskItem>> GetTasksAsync()
        {
            var query = _container.GetItemQueryIterator<TaskItem>();
            var results = new List<TaskItem>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
        public async Task<bool> UpdateTaskAsync(string id, TaskItem task)
        {
            var container = _container;
            try
            {
                await container.ReplaceItemAsync(task, id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    }
}
