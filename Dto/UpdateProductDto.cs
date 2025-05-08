namespace BackendProject2.Dto
{
    public class UpdateProductDto
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal? Rating { get; set; }
        public decimal? ProductPrice { get; set; }
        public decimal? OfferPrize { get; set; }
        public int? CategoryId { get; set; }
    }
}
