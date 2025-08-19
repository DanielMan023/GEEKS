using System.ComponentModel.DataAnnotations;

namespace GEEKS.Models
{
    public class User : Base
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        public string? PhoneNumber { get; set; }
        
        [Required]
        public string State { get; set; } = "Active";
        
        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
    }
}
