using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ItiUmplemFrigiderul.Models
{
    public class Cart
    {
        [Key]
        public int ProductId { get; set; }
        [Key]
        public int UserId { get; set; }
        [Key]
        public int FarmId { get; set; }

        public virtual IdentityUser User{ get; set; }
        public virtual Farm Farm { get; set; }
        public virtual Product Product { get; set; }

        public int? OrderId { get; set; }

        public virtual Order? Order{ get; set; }


        [Required(ErrorMessage = "Cantitatea este obligatorie")]
        public virtual float Quantity { get; set; }
    }
}
