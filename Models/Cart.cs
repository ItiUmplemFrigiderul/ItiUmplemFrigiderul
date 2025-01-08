using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ItiUmplemFrigiderul.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int FarmProductId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User{ get; set; }
        public virtual FarmProduct FarmProduct { get; set; }

        [Required(ErrorMessage = "Cantitatea este obligatorie")]
        public virtual int Quantity { get; set; }
    }
}
