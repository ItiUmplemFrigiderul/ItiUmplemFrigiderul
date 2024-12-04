using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItiUmplemFrigiderul.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }


        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual Farm? Farm { get; set; }
        public virtual Cart Cart { get; set; }


        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}
