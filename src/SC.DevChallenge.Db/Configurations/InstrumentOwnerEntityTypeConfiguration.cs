using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SC.DevChallenge.Db.Models;

namespace SC.DevChallenge.Db.Configurations
{
    public class InstrumentOwnerEntityTypeConfiguration : IEntityTypeConfiguration<InstrumentOwner>
    {
        public void Configure(EntityTypeBuilder<InstrumentOwner> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
