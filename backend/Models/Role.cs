using System.ComponentModel.DataAnnotations;

namespace GEEKS.Models
{
    public class Role : Base
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Scope { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public string State { get; set; } = "Active";
        
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
