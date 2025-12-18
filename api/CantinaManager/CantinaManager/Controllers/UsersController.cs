using CantinaManager.Data;
using CantinaManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace CantinaManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository _repository;

        public UsersController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _repository.GetAllUsersAsync();
            var result = users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.FullName,
                u.ProfilePictureUrl,
                u.CreatedAt,
                Tasks = u.UserTasks.Select(t => new { t.Id, t.Title, t.Description, t.CreatedAt })
            });

            return Ok(result);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.ProfilePictureUrl,
                user.CreatedAt,
                Tasks = user.UserTasks.Select(t => new { t.Id, t.Title, t.Description, t.CreatedAt })
            });
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("Invalid user data");

            var createdUser = await _repository.CreateUserAsync(user);

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var existingUser = await _repository.GetUserByIdAsync(id);
            if (existingUser == null)
                return NotFound("User not found");

            existingUser.FullName = updatedUser.FullName;
            existingUser.ProfilePictureUrl = updatedUser.ProfilePictureUrl;
            existingUser.Email = updatedUser.Email;
            existingUser.UserName = updatedUser.UserName;

            await _repository.UpdateUserAsync(existingUser);

            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _repository.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
