using System.ComponentModel.DataAnnotations;

namespace BackendProject2.Dto
{
    public class OrderViewDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        [Required]
        public string? OrderString { get; set; }

        public string? ProductImage { get; set; }

        [Required]
        public string? TransactionId { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
