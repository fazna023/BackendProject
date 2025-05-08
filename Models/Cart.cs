namespace BackendProject2.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<CartItems> _Items { get; set; }
        public virtual User? _User { get; set; }
    }
}
