using Application.Repositories;
using DAL.Data;
using DAL.Data.Connection;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Boundary.DALConfig;

public static class SqlConfiguration
{
    public static void AddSql(this IServiceCollection services, string connectionString)
    {
        var connectionFactory = new ConnectionFactory(connectionString);
        services.AddTransient<IConnectionFactory>(sp => connectionFactory);

        services.AddDbContext<eZboriDbContext>(opt => opt.UseSqlServer(connectionString));

        // Repository config
        services.RegisterRepositories();
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPresidencyRepository, PresidencyRepository>();
        services.AddScoped<IMunicipalityServiceRepository, MunicipalityServiceRepository>();
        services.AddScoped<IStateRepository, StateRepository>();
        services.AddScoped<IEntityRepository, EntityRepository>();
        services.AddScoped<ICantonRepository, CantonRepository>();
        services.AddScoped<IMunicipalityRepository, MunicipalityRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IAnalysisRepository, AnalysisRepository>();
        services.AddScoped<IForecastedResultRepository, ForecastedResultRepository>();
        services.AddScoped<ISavedSearchRepository, SavedSearchRepository>();
        services.AddScoped<IElectionCycleRepository, ElectionCycleRepository>();
        services.AddScoped<IImportJobRepository, ImportJobRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }
}