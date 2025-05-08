namespace BackendProject2.Dto
{
    public class CartViewDto
    {
        public int cartviewid { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }

        public int? Price { get; set; }

        public string? ProductImage { get; set; }

        public int? TotalAmount { get; set; }

        public int? OrginalPrize { get; set; }

        public int? Quantity { get; set; }
    }
}
