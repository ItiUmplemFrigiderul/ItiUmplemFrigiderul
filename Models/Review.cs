using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ItiUmplemFrigiderul.Models
{
    public class Review
    {
        [Key]
        public int FarmId { get; set; }
        [Key]
        public int ProductId { get; set; }
        [Key]
        public int UserId { get; set; }

        public virtual Farm Farm { get; set; }
        public virtual Product Product { get; set; }
        public virtual IdentityUser User { get; set; }

        [Required(ErrorMessage = "Nota este obligatorie")]
        public int Rating { get; set; }
        public string? Content { get; set; }

    }
}
