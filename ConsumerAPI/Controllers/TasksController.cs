using ConsumerAPI.Services;
using CosmosDB_Simple_API.Models;
using CosmosDB_Simple_API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDB_Simple_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskCacheService _taskCacheService;

        public TasksController(ITaskRepository taskRepository, ITaskCacheService taskCacheService)
        {
            _taskRepository = taskRepository;
            _taskCacheService = taskCacheService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
        {
            var tasks = await _taskCacheService.GetAllTasksAsync(
                async () => await _taskRepository.GetTasksAsync()
            );
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetById(string id)
        {
            var task = await _taskCacheService.GetTaskByIdAsync(
                id,
                async () => await _taskRepository.GetTaskAsync(id)
            );

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> Create(TaskItem task)
        {
            task.Id = Guid.NewGuid().ToString();
            await _taskRepository.AddTaskAsync(task);

            await _taskCacheService.CacheTaskAsync(task);
            await _taskCacheService.InvalidateAllTasksCache();

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, TaskItem task)
        {
            var updated = await _taskRepository.UpdateTaskAsync(id, task);
            if (!updated)
                return NotFound();

            await _taskCacheService.InvalidateTaskCache(id);
            await _taskCacheService.InvalidateAllTasksCache();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _taskRepository.DeleteTaskAsync(id);
            if (!deleted)
                return NotFound();

            await _taskCacheService.InvalidateTaskCache(id);
            await _taskCacheService.InvalidateAllTasksCache();

            return NoContent();
        }
    }
}
