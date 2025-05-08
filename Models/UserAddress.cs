namespace BackendProject2.Models
{
    public class UserAddress
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public int UserId { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string HomeAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string PostalCode { get; set; }
        public virtual User _UserAd { get; set; }

        public ICollection<Order> _Orders { get; set; }

    }
}