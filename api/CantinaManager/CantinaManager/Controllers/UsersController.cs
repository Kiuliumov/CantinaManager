using CantinaManager.Data;
using CantinaManager.Models;
using CantinaManager.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CantinaManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _jwtService;

        public UsersController(IRepository repository, UserManager<User> userManager, ITokenService jwtService)
        {
            _repository = repository;
            _userManager = userManager;
            _jwtService = jwtService;
        }

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
                Tasks = u.UserTasks.Select(t => new { t.Id, t.Title, t.Content, t.CreatedAt, t.DueDate })
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found");

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.ProfilePictureUrl,
                user.CreatedAt,
                Tasks = user.UserTasks.Select(t => new { t.Id, t.Title, t.Content, t.CreatedAt, t.DueDate })
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null) return BadRequest("Invalid user data");

            var createdUser = await _repository.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var existingUser = await _repository.GetUserByIdAsync(id);
            if (existingUser == null) return NotFound("User not found");

            existingUser.FullName = updatedUser.FullName;
            existingUser.ProfilePictureUrl = updatedUser.ProfilePictureUrl;
            existingUser.Email = updatedUser.Email;
            existingUser.UserName = updatedUser.UserName;

            await _repository.UpdateUserAsync(existingUser);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _repository.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.Username);
            if (existingUser != null) return BadRequest("Username already exists");

            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var accessToken = await _jwtService.GenerateAccessTokenAsync(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            await _repository.CreateRefreshTokenAsync(refreshTokenEntity);

            return Ok(new { accessToken, refreshToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials");

            var accessToken = await _jwtService.GenerateAccessTokenAsync(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            await _repository.CreateRefreshTokenAsync(refreshTokenEntity);

            return Ok(new { accessToken, refreshToken });
        }
    }

    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
