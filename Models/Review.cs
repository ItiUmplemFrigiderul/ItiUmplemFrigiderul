using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ItiUmplemFrigiderul.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int FarmProductId { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessage = "Produsul este obligatoriu")]
        public virtual FarmProduct? FarmProduct { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "The rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string? Content { get; set; }
        public DateTime Date {  get; set; }

    }
}
