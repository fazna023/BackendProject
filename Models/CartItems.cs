using System.ComponentModel.DataAnnotations;

namespace BackendProject2.Models
{
    public class CartItems
    {
        [Key]
        public int? Id { get; set; }
        public int ProductId { get; set; }
        public int CartId { get; set; }
        public int ProductQty { get; set; }
        public virtual Cart? _Cart { get; set; }
        public virtual Product? _Product { get; set; }
    }
}
