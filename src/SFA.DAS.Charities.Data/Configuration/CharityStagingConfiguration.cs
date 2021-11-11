using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Charities.Domain.Entities;

namespace SFA.DAS.Charities.Data.Configuration
{
    internal class CharityStagingConfiguration : IEntityTypeConfiguration<CharityStaging>
    {
        public void Configure(EntityTypeBuilder<CharityStaging> builder)
        {
            builder.HasMany(c => c.Trustees).WithOne().HasPrincipalKey(x => x.Id).HasForeignKey(x => x.CharityId);
        }
    }
}
