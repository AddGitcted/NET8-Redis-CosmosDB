using CosmosDB_Simple_API.Models;

namespace ConsumerAPI.Services
{
    public interface ITaskCacheService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync(Func<Task<IEnumerable<TaskItem>>> factory);
        Task<TaskItem?> GetTaskByIdAsync(string id, Func<Task<TaskItem?>> factory);
        Task CacheTaskAsync(TaskItem task);
        Task InvalidateAllTasksCache();
        Task InvalidateTaskCache(string id);
    }

    public class TaskCacheService : ITaskCacheService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<TaskCacheService> _logger;
        private const string CACHE_KEY_ALL_TASKS = "all_tasks";
        private const string CACHE_KEY_TASK = "task_";

        public TaskCacheService(ICacheService cacheService, ILogger<TaskCacheService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync(Func<Task<IEnumerable<TaskItem>>> factory)
        {
            var cachedTasks = await _cacheService.GetOrCreateAsync(
                CACHE_KEY_ALL_TASKS,
                async () =>
                {
                    _logger.LogInformation("Fetching data from the database.");
                    return await factory();
                },
                TimeSpan.FromMinutes(5)
            );

            if (cachedTasks != null)
            {
                _logger.LogInformation("Data retrieved from cache.");
            }

            return cachedTasks;
        }

        public async Task<TaskItem?> GetTaskByIdAsync(string id, Func<Task<TaskItem?>> factory)
        {
            return await _cacheService.GetOrCreateAsync(
                $"{CACHE_KEY_TASK}{id}",
                factory,
                TimeSpan.FromMinutes(10)
            );
        }

        public async Task CacheTaskAsync(TaskItem task)
        {
            await _cacheService.SetAsync($"{CACHE_KEY_TASK}{task.Id}", task);
        }

        public async Task InvalidateAllTasksCache()
        {
            await _cacheService.RemoveAsync(CACHE_KEY_ALL_TASKS);
        }

        public async Task InvalidateTaskCache(string id)
        {
            await _cacheService.RemoveAsync($"{CACHE_KEY_TASK}{id}");
        }
    }
}
