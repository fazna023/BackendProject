namespace BackendProject2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal Total { get; set; }
        public string OrderString { get; set; }
        public string? ProductImage { get; set; }

        public string TransactionId { get; set; }


        public virtual UserAddress _UserAd { get; set; }
        public virtual User _User { get; set; }
        public virtual List<OrderItems> Items { get; set; }
    }
}