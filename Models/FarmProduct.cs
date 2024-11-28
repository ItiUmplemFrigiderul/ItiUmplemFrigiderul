using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItiUmplemFrigiderul.Models
{
    public class FarmProduct
    {
        [Key]
        public int FarmId { get; set; }
        [Key]
        [Required(ErrorMessage = "Produsul este obligatoriu")]

        public int ProductId { get; set; }
        public virtual Farm Farm { get; set; }
        public virtual Product  Product { get; set; }

        [Required(ErrorMessage = "Pretul este obligatoriu")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Stocul este obligatoriu")]
        public virtual int Stock { get; set; }
        public bool Verified { get; set; }//default false

        [NotMapped]
        public IEnumerable<SelectListItem>? Prod { get; set; }
    }
}
