using Microsoft.EntityFrameworkCore;

namespace RestoSmart.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<SalesHeader> SalesHeaders { get; set; }
        public DbSet<SalesDetail> SalesDetails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<RawMaterial> RawMaterials { get; set; } 
        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<RestaurantTable> RestaurantTables { get; set; }
    }
}