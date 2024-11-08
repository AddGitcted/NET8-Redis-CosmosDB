using CosmosDB_Simple_API.Models;
using System.Collections.Concurrent;

namespace CosmosDB_Simple_API.Repositories
{
    public class MockTaskRepository : ITaskRepository
    {
        private readonly ConcurrentDictionary<string, TaskItem> _tasks = new();

        public async Task AddTaskAsync(TaskItem task)
        {
            _tasks[task.Id] = task;
            await Task.CompletedTask;
        }

        public async Task DeleteTaskAsync(string id)
        {
            _tasks.TryRemove(id, out _);
            await Task.CompletedTask;
        }

        public async Task<TaskItem?> GetTaskAsync(string id)
        {
            _tasks.TryGetValue(id, out var task);
            return await Task.FromResult(task);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksAsync()
        {
            return await Task.FromResult(_tasks.Values);
        }

        public async Task UpdateTaskAsync(string id, TaskItem task)
        {
            _tasks[id] = task;
            await Task.CompletedTask;
        }
    }
}
