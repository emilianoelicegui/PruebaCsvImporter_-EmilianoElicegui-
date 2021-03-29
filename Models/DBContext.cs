using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ModelConfig(builder);
        }

        private void ModelConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.PointOfSale).IsRequired();
                entity.Property(x => x.Product).IsRequired();
                entity.Property(x => x.Date).IsRequired();
                entity.Property(x => x.Stock).IsRequired();
            });

            
        }
    }
}
