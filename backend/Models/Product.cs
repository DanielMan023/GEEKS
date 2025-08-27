using System.ComponentModel.DataAnnotations;

namespace GEEKS.Models
{
    public class Product : Base
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? ShortDescription { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal Price { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "El precio con descuento debe ser mayor o igual a 0")]
        public decimal? DiscountPrice { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser mayor o igual a 0")]
        public int MinStock { get; set; } = 5;
        
        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? MainImage { get; set; }
        
        public List<string> Images { get; set; } = new List<string>();
        
        [Required]
        public int CategoryId { get; set; }
        
        public virtual Category Category { get; set; } = null!;
        
        [StringLength(100)]
        public string? Brand { get; set; }
        
        [Required]
        public string State { get; set; } = "Active";
        
        public bool IsFeatured { get; set; } = false;
        
        [Range(0, double.MaxValue, ErrorMessage = "El peso debe ser mayor o igual a 0")]
        public decimal Weight { get; set; } = 0;
        
        [Range(0, double.MaxValue, ErrorMessage = "La longitud debe ser mayor o igual a 0")]
        public decimal Length { get; set; } = 0;
        
        [Range(0, double.MaxValue, ErrorMessage = "El ancho debe ser mayor o igual a 0")]
        public decimal Width { get; set; } = 0;
        
        [Range(0, double.MaxValue, ErrorMessage = "La altura debe ser mayor o igual a 0")]
        public decimal Height { get; set; } = 0;
    }
}

