using Microsoft.EntityFrameworkCore;
using StronglyTypedIdDemo.Infrastructure.EntityFrameworkCore;

namespace StronglyTypedIdDemo.Data
{
    public class StronglyTypedIdDemoDbContext : DbContext
    {
        public StronglyTypedIdDemoDbContext(DbContextOptions<StronglyTypedIdDemoDbContext> options)
           : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>().HasKey(t => t.Id);

            modelBuilder.AddStronglyTypedIdConversions();
        }
    }
}
