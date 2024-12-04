using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ItiUmplemFrigiderul.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string Adress { get; set; }

        public int Total { get; set; }

        public DateTime Date { get; set; }
        public virtual ICollection<ProductOrder>? ProductOrders { get; set; }


    }
}
