using System.ComponentModel.DataAnnotations;

namespace GEEKS.Models
{
    public class Category : Base
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? Image { get; set; }
        
        [Required]
        public string State { get; set; } = "Active";
        
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

