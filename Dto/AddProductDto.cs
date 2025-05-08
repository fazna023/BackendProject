using System.ComponentModel.DataAnnotations;

namespace BackendProject2.Dto
{
    public class AddProductDto
    {
        [Required]
        public string? ProductName { get; set; }

        [Required]
        public string? ProductDescription { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }
        [Required]
        public decimal OfferPrize { get; set; }
        [Required]
        public string CategoryName { get; set; }

      
        public int CategoryId { get; set; }

     

    }
}
