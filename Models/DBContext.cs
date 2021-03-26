using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class DBContext : DbContext
    {
        public DBContext()
        {
        }

        //Constructor con parametros para la configuracion
        public DBContext(DbContextOptions options)
        : base(options)
        {
        }

        //Sobreescribimos el metodo OnConfiguring para hacer los ajustes que queramos en caso de
        //llamar al constructor sin parametros
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //En caso de que el contexto no este configurado, lo configuramos mediante la cadena de conexion
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=CsvImporter;Trusted_Connection=True;MultipleActiveResultSets=true");
                //optionsBuilder.UseSqlServer("Server=localhost;Database=postefcore;Uid=root;Pwd=root;");
            }
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
