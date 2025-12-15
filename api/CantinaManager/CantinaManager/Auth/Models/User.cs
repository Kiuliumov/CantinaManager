using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CantinaManager.Models
{
    public class User : IdentityUser
    {
        // Extra profile fields
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property: tasks assigned to the user
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
