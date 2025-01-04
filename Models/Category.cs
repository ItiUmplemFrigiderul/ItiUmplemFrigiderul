using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ItiUmplemFrigiderul.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string CategoryName { get; set; }
        [NotMapped]
        public IEnumerable<Product>? Prods { get; set; }
        public virtual ICollection<Product>? Products { get; set; }

    }
}
