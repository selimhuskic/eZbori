using DAL.Data.EntityConfiguration;
using UserRole = Application.Models.UserRole;

namespace DAL.Data;

public class eZboriDbContext : DbContext
{
    public eZboriDbContext(DbContextOptions<eZboriDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SeededCheckConfiguration());
        modelBuilder.ApplyConfiguration(new PresidencyOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new PresidencyResultsConfiguration());
        modelBuilder.ApplyConfiguration(new MunicipalityConfiguration());
        modelBuilder.ApplyConfiguration(new PresidencyMunicipalOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new PresidencyMunicipalResultsConfiguration());
        modelBuilder.ApplyConfiguration(new StateElectoralUnitOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new StateElectoralUnitPartyConfiguration());
        modelBuilder.ApplyConfiguration(new StateMunicipalOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new StateMunicipalPartyConfiguration());
        modelBuilder.ApplyConfiguration(new EntityElectoralUnitOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new EntityElectoralUnitPartyConfiguration());
        modelBuilder.ApplyConfiguration(new EntityPresidentOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new EntityPresidentMunicipalCandidateConfiguration());
        modelBuilder.ApplyConfiguration(new EntityMunicipalOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new EntityMunicipalPartyConfiguration());
        modelBuilder.ApplyConfiguration(new CantonElectoralUnitOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new CantonElectoralUnitPartyConfiguration());
        modelBuilder.ApplyConfiguration(new CantonMunicipalOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new CantonMunicipalPartyConfiguration());
        modelBuilder.ApplyConfiguration(new MunicipalityCandidateDetailsConfiguration());
        modelBuilder.ApplyConfiguration(new MunicipalityCandidateOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new MunicipalityCouncilOverviewConfiguration());
        modelBuilder.ApplyConfiguration(new MunicipalityCouncilPartyConfiguration());
        modelBuilder.ApplyConfiguration(new MunicipalityCouncilMinorityConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new ForecastedResultConfiguration());
        modelBuilder.ApplyConfiguration(new SavedSearchConfiguration());
        modelBuilder.ApplyConfiguration(new ElectionCycleConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<SeedingCheck> SeededCheck { get; set; }
    public DbSet<PresidencyOverview> PresidencyOverview { get; set; }
    public DbSet<PresidencyResults> PresidencyResults { get; set; }
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<PresidencyMunicipalOverview> PresidencyMunicipalOverview { get; set; }
    public DbSet<PresidencyMunicipalResults> PresidencyMunicipalResults { get; set; }
    public DbSet<StateElectoralUnitOverview> StateElectoralUnitOverview { get; set; }
    public DbSet<StateElectoralUnitParty> StateElectoralUnitParty { get; set; }
    public DbSet<StateMunicipalOverview> StateMunicipalOverview { get; set; }
    public DbSet<StateMunicipalParty> StateMunicipalParty { get; set; }
    public DbSet<EntityElectoralUnitOverview> EntityElectoralUnitOverview { get; set; }
    public DbSet<EntityElectoralUnitParty> EntityElectoralUnitParty { get; set; }
    public DbSet<EntityPresidentOverview> EntityPresidentOverview { get; set; }
    public DbSet<EntityPresidentMunicipalCandidate> EntityPresidentMunicipalCandidate { get; set; }
    public DbSet<EntityMunicipalOverview> EntityMunicipalOverview { get; set; }
    public DbSet<EntityMunicipalParty> EntityMunicipalParty { get; set; }
    public DbSet<CantonElectoralUnitOverview> CantonElectoralUnitOverview { get; set; }
    public DbSet<CantonElectoralUnitParty> CantonElectoralUnitParties { get; set; }
    public DbSet<CantonMunicipalOverview> CantonMunicipalOverview { get; set; }
    public DbSet<CantonMunicipalParty> CantonMunicipalParties { get; set; }
    public DbSet<MunicipalityCandidateDetails> MunicipalityCandidateDetails { get; set; }
    public DbSet<MunicipalityCandidateOverview> MunicipalityCandidateOverview { get; set; }
    public DbSet<MunicipalityCouncilOverview> MunicipalityCouncilOverview { get; set; }
    public DbSet<MunicipalityCouncilParty> MunicipalityCouncilParties { get; internal set; }
    public DbSet<MunicipalityCouncilMinority> MunicipalityCouncilMinorities { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ForecastedResult> ForecastedResults { get; set; }
    public DbSet<SavedSearch> SavedSearches { get; set; }
    public DbSet<ElectionCycle> ElectionCycles { get; set; }
    public DbSet<ImportJob> ImportJobs { get; set; }
    public DbSet<Notification> Notifications { get; set; }
}
