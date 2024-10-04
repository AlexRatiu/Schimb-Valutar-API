using Microsoft.EntityFrameworkCore;
using Schimb_Valutar_API.DAL.DBO;

namespace Schimb_Valutar_API.DAL
{
    public class CurrencyDbContext : DbContext
    {
        public CurrencyDbContext(DbContextOptions<CurrencyDbContext> dbContextOptions) : base(dbContextOptions) { }
        public DbSet<Currency> Currency { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Currency>()
                .HasKey(x => x.id);

            modelBuilder.Entity<Currency>()
                .Property(x => x.id);

            modelBuilder.Entity<Currency>()
                .Property(x => x.abreviation);

            modelBuilder.Entity<Currency>()
                .Property(x => x.value)
                .HasPrecision(18,4);
        }
    }
}
