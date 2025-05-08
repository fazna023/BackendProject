namespace BackendProject2.Models
{
    public class WishList
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public virtual User _User { get; set; }
        public virtual Product _Product { get; set; }

    }
}
