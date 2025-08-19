using System.ComponentModel.DataAnnotations;

namespace GEEKS.Models
{
    public abstract class Base
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime CreatedAtDateTime { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAtDateTime { get; set; }
        
        public int? IdUserCreated { get; set; }
        
        public int? IdUserUpdated { get; set; }
    }
}
