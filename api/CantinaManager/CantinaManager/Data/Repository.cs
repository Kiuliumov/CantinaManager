using CantinaManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserTasks)
                .ToListAsync();
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
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

        public async Task<List<UserTask>> GetAllTasksAsync()
        {
            return await _context.UserTasks
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
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

        public async Task<int> GetTaskCountByUserIdAsync(string userId)
        {
            return await _context.UserTasks.CountAsync(t => t.UserId == userId);
        }

        public async Task UpdateTaskAsync(UserTask task)
        {
            _context.UserTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var task = await _context.UserTasks.FindAsync(taskId);
            if (task == null) return;

            _context.UserTasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllTasksForUserAsync(string userId)
        {
            var tasks = await _context.UserTasks
                .Where(t => t.UserId == userId)
                .ToListAsync();

            _context.UserTasks.RemoveRange(tasks);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<List<RefreshToken>> GetRefreshTokensByUserIdAsync(string userId)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IsRefreshTokenValidAsync(string token)
        {
            return await _context.RefreshTokens.AnyAsync(rt =>
                rt.Token == token &&
                !rt.IsRevoked &&
                rt.Expires > DateTime.UtcNow);
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken == null) return;

            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllRefreshTokensForUserAsync(string userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpiredRefreshTokensAsync()
        {
            var expiredTokens = await _context.RefreshTokens
                .Where(rt => rt.Expires <= DateTime.UtcNow)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
