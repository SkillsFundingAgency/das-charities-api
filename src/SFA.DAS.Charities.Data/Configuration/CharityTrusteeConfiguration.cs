using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Charities.Domain.Entities;

namespace SFA.DAS.Charities.Data.Configuration
{
    internal class CharityTrusteeConfiguration : IEntityTypeConfiguration<CharityTrustee>
    {
        public void Configure(EntityTypeBuilder<CharityTrustee> builder)
        {
            builder.ToTable(nameof(CharityTrustee));
        }
    }
}
