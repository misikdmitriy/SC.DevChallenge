using Microsoft.EntityFrameworkCore;
using SC.DevChallenge.Db.Configurations;
using SC.DevChallenge.Db.Models;

namespace SC.DevChallenge.Db.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<InstrumentOwner> InstrumentOwners { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<PriceModel> PriceModels { get; set; }
        public DbSet<ContentHistory> ContentHistories { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new InstrumentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new InstrumentOwnerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PortfolioEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PriceModelEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ContentHistoryEntityTypeConfiguration());
        }
    }
}
