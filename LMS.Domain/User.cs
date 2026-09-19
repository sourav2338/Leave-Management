using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; } 
        public string? Role { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public int UpdatedBy { get; set; }
        public DateTime UpdatedON { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
