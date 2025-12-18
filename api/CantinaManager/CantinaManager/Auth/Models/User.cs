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
        
        public ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();

    }
}
