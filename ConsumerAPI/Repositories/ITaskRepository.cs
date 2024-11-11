using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDB_Simple_API.Models;

namespace CosmosDB_Simple_API.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetTasksAsync();
        Task<TaskItem> GetTaskAsync(string id);
        Task AddTaskAsync(TaskItem task);
        Task<bool> UpdateTaskAsync(string id, TaskItem task);
        Task<bool> DeleteTaskAsync(string id);
    }
}
