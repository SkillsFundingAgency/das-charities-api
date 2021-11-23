using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Charities.Domain.Entities;

namespace SFA.DAS.Charities.Data.Configuration
{
    internal class CharityTrusteeStagingConfiguration : IEntityTypeConfiguration<CharityTrusteeStaging>
    {
        public void Configure(EntityTypeBuilder<CharityTrusteeStaging> builder)
        {
            builder.ToTable(nameof(CharityTrusteeStaging));
        }
    }
}
