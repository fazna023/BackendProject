namespace BackendProject2.Dto
{
    public class CreateOrderDto
    {
        public int AddId { get; set; }
        public decimal? Total { get; set; }

        public string? OrderString { get; set; }

        public string? TransactionId
        {
            get; set;
        }
    }
}
