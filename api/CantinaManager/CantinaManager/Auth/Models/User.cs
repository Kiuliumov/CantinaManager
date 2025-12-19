using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CantinaManager.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        
        public string? ProfilePictureUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
        
        public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();

    }
}
