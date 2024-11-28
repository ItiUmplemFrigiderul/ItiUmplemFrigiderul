using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ItiUmplemFrigiderul.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int UserId { get; set; }
        public virtual IdentityUser User { get; set; }

        public string Adress { get; set; }

        public int Total { get; set; }

        public DateTime Date { get; set; }


    }
}
