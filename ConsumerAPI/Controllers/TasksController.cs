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

        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<TaskItem>> Get()
        {
            return await _taskRepository.GetTasksAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> Get(string id)
        {
            var task = await _taskRepository.GetTaskAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return task;
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> Post([FromBody] TaskItem task)
        {
            task.Id = Guid.NewGuid().ToString();
            await _taskRepository.AddTaskAsync(task);
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] TaskItem task)
        {
            var existingTask = await _taskRepository.GetTaskAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }
            await _taskRepository.UpdateTaskAsync(id, task);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingTask = await _taskRepository.GetTaskAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }
            await _taskRepository.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}
