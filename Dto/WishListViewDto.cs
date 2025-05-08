namespace BackendProject2.Dto
{
    public class WishListViewDto
    {
        public int? Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal? Price { get; set; }
        public decimal? OfferPrice { get; set; }
        public string? CategoryName { get; set; }
        public string? ProductImage { get; set; }
    }
}
