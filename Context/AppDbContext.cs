using BackendProject2.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendProject2.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    
        public DbSet<User> users { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Cart> cart { get; set; }
        public DbSet<CartItems> cartItems { get; set; }
        public DbSet<WishList> whishlist { get; set; }
        public DbSet<Order> order { get; set; }
        public DbSet<OrderItems> orderItems { get; set; }
        public DbSet<UserAddress> userAddress { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(x => x.Role)
                .HasDefaultValue("User");

            modelBuilder.Entity<User>()
                .Property(i => i.isBlocked)
                .HasDefaultValue(false);

            


            modelBuilder.Entity<Product>()
             .Property(pr => pr.ProductPrice)
             .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(pr => pr.offerPrize)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(pr => pr.Rating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<Product>()
                .Property(pr => pr.StockId)
                .HasDefaultValue(50);

            modelBuilder.Entity<Product>()
                .HasOne(a => a._Category)
                .WithMany(b => b._Produts)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<CartItems>()
             .Property(a => a.ProductQty)
             .HasDefaultValue(1);

            modelBuilder.Entity<Cart>()
                .HasOne(a => a._User)
                .WithOne(b => b._Cart)
                .HasForeignKey<Cart>(c => c.UserId);

            modelBuilder.Entity<CartItems>()
                .HasOne(a => a._Cart)
                .WithMany(b => b._Items)
                .HasForeignKey(c => c.CartId);

            modelBuilder.Entity<CartItems>()
                .HasOne(a => a._Product)
                .WithMany(b => b.CartItems)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishList>()
                .HasOne(a => a._User)
                .WithMany(b => b._WishLists)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<WishList>()
                .HasOne(a => a._Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(a=>a._User)
                .WithMany(b=>b._Orders)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
               .HasOne(a => a._UserAd)
               .WithMany(b => b._Orders)
               .HasForeignKey(c => c.AddressId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
               .Property(pr => pr.OrderStatus)
               .HasDefaultValue("OderPlaced");

            modelBuilder.Entity<Order>()
                .Property(pr => pr.Total)
                .HasPrecision(30, 2);

            modelBuilder.Entity<OrderItems>()
                .HasOne(a => a._Order)
                .WithMany(b => b.Items)
                .HasForeignKey(c => c.OrderId);

            modelBuilder.Entity<OrderItems>()
                .HasOne(a => a.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId);

            modelBuilder.Entity<OrderItems>()
                .Property(pr => pr.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserAddress>()
           .HasOne(a => a._UserAd)
           .WithMany(b => b._UserAddresses)
           .HasForeignKey(c => c.UserId);
        }



    }
}