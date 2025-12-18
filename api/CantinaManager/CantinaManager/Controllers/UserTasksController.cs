using CantinaManager.Data;
using CantinaManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CantinaManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserTasksController : ControllerBase
    {
        private readonly IRepository _repository;

        public UserTasksController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks(int page = 1, int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var allTasks = await _repository.GetAllTasksAsync();
            var totalCount = allTasks.Count;

            var pagedTasks = allTasks
                .OrderBy(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var result = new
            {
                page,
                pageSize,
                totalCount,
                totalPages,
                tasks = pagedTasks.Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Content,
                    t.CreatedAt,
                    t.DueDate,
                    t.UserId
                })
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _repository.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            return Ok(new
            {
                task.Id,
                task.Title,
                task.Content,
                task.CreatedAt,
                task.DueDate,
                task.UserId
            });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTasksByUser(string userId, int page = 1, int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var allTasks = await _repository.GetTasksByUserIdAsync(userId, 0, int.MaxValue);
            var totalCount = allTasks.Count;

            var pagedTasks = allTasks
                .OrderBy(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var result = new
            {
                page,
                pageSize,
                totalCount,
                totalPages,
                tasks = pagedTasks.Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Content,
                    t.CreatedAt,
                    t.DueDate,
                    t.UserId
                })
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            var task = new UserTask
            {
                Title = dto.Title,
                Content = dto.Content,
                DueDate = dto.DueDate,
                UserId = dto.UserId
            };

            var createdTask = await _repository.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var existingTask = await _repository.GetTaskByIdAsync(id);
            if (existingTask == null) return NotFound();

            existingTask.Title = dto.Title;
            existingTask.Content = dto.Content;
            existingTask.DueDate = dto.DueDate;

            await _repository.UpdateTaskAsync(existingTask);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _repository.DeleteTaskAsync(id);
            return NoContent();
        }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
