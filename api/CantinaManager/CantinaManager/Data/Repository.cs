using CantinaManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CantinaManager.Data
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.UserTasks)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.UserTasks)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserTasks)
                .ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserTask> CreateTaskAsync(UserTask task)
        {
            _context.UserTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<UserTask?> GetTaskByIdAsync(int taskId)
        {
            return await _context.UserTasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task<List<UserTask>> GetTasksByUserIdAsync(string userId, int offset = 0, int limit = 10)
        {
            return await _context.UserTasks
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task UpdateTaskAsync(UserTask task)
        {
            _context.UserTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var task = await _context.UserTasks.FindAsync(taskId);
            if (task != null)
            {
                _context.UserTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
}
