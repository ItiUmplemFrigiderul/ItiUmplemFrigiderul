using System.ComponentModel.DataAnnotations;

namespace ItiUmplemFrigiderul.Models
{
    public class ProductOrder
    {
        [Key]
        public int Id { get; set; }
        public int FarmProductId { get; set; }
        public virtual FarmProduct FarmProduct { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public virtual float Quantity { get; set; }
    }
}
