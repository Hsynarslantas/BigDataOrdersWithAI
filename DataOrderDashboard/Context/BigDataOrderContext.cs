using DataOrderDashboard.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.Context
{
    public class BigDataOrderContext:DbContext
    {
        public BigDataOrderContext(DbContextOptions<BigDataOrderContext> options):base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
