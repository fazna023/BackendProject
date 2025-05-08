

namespace BackendProject2.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; } = "User";
        public bool isBlocked { get; set; }
        public virtual Cart _Cart { get; set; }
        public virtual ICollection<WishList> _WishLists { get; set; }
        
        public virtual ICollection<Order> _Orders { get; set; }
        public virtual ICollection<UserAddress> _UserAddresses { get; set; }
    }
}
