using BackendProject2.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendProject2.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    
        public DbSet<User> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(x => x.Role)
                .HasDefaultValue("User");

            modelBuilder.Entity<User>()
                .Property(i => i.isBlocked)
                .HasDefaultValue(false);
        }



    }
}