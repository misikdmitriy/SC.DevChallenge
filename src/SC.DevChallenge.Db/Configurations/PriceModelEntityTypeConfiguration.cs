using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SC.DevChallenge.Db.Models;

namespace SC.DevChallenge.Db.Configurations
{
    public class PriceModelEntityTypeConfiguration : IEntityTypeConfiguration<PriceModel>
    {
        public void Configure(EntityTypeBuilder<PriceModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Instrument).WithMany()
                .HasForeignKey(x => x.InstrumentId);
            builder.HasOne(x => x.InstrumentOwner).WithMany()
                .HasForeignKey(x => x.InstrumentOwnerId);
            builder.HasOne(x => x.Portfolio).WithMany()
                .HasForeignKey(x => x.PortfolioId);
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Price).IsRequired();
        }
    }
}
