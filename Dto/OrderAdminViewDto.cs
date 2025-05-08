namespace BackendProject2.Dto
{
    public class OrderAdminViewDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        public string TransactionId { get; set; }

        public string? OrderString { get; set; }
        public string? UserAddress { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }
    }
}
