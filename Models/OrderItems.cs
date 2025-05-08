namespace BackendProject2.Models
{
    public class OrderItems
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public string? ProductImage { get; set; }
        public virtual Product Product { get; set; }
        public virtual Order _Order { get; set; }
    }
}
