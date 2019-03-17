using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SC.DevChallenge.Db.Models;

namespace SC.DevChallenge.Db.Configurations
{
    public class ContentHistoryEntityTypeConfiguration : IEntityTypeConfiguration<ContentHistory>
    {
        public void Configure(EntityTypeBuilder<ContentHistory> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Hash).IsRequired();
            builder.Property(x => x.LastUpdate).IsRequired();
        }
    }
}
