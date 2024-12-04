using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItiUmplemFrigiderul.Models
{
    public class FarmProduct
    {
        [Key]
        public int Id { get; set; }
        public int? FarmId { get; set; }
        
        [Required(ErrorMessage = "Produsul este obligatoriu")]
        public int? ProductId { get; set; }
        public virtual Farm? Farm { get; set; }
        public virtual Product?  Product { get; set; }

        [Required(ErrorMessage = "Pretul este obligatoriu")]
        public int Price { get; set; }


        [Required(ErrorMessage = "Stocul este obligatoriu")]
        public virtual int Stock { get; set; }
        public bool Verified { get; set; }//default false
        public float Rating { get; set; }

        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<ProductOrder>? ProductOrders { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Prod { get; set; }
    }
}
