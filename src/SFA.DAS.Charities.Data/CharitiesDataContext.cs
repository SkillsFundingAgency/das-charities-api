using Microsoft.EntityFrameworkCore;
using SFA.DAS.Charities.Domain.Entities;

namespace SFA.DAS.Charities.Data
{
    public class CharitiesDataContext : DbContext
    {
        public DbSet<Charity> Charities { get; set; }
        public DbSet<CharityStaging> CharitiesStaging { get; set; }
        public DbSet<CharityTrustee> CharityTrustees { get; set; }
        public DbSet<CharityTrusteeStaging> CharityTrusteesStaging { get; set; }

        public CharitiesDataContext(DbContextOptions<CharitiesDataContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CharitiesDataContext).Assembly);
        }
    }
}
