using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItiUmplemFrigiderul.Models
{
    public class Farm
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(100, ErrorMessage = "Numele nu poate avea mai mult de 100 de caractere")]
        [MinLength(5, ErrorMessage = "Numele trebuie sa aibă mai mult de 5 caractere")]
        public string  Name { get; set; }

        [Required(ErrorMessage = "Numarul de telefon este obligatoriu")]
        [StringLength(10, ErrorMessage = "Numar invalid")]
        [MinLength(10, ErrorMessage = "Numar invalid")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Adresa este obligatorie")]
        public string Adress { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<FarmProduct>? FarmProducts { get; set; }

    }
}
