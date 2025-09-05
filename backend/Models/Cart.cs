using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GEEKS.Models
{
    public class Cart : Base
    {
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Propiedades calculadas
        [NotMapped]
        public decimal Total => CartItems?.Sum(item => item.Subtotal) ?? 0;

        [NotMapped]
        public int TotalItems => CartItems?.Sum(item => item.Quantity) ?? 0;
    }
}
