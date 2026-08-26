using Application.Options;
using Application.Services;
using Boundary.Messaging;
using DAL.MachineLearning;
using DAL.Mapping;
using DAL.MappingServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boundary.ApplicationServicesConfig;

public static class ApplicationServicesConfiguration
{
    public static void RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<EZboriMapper>();
        services.AddScoped<IPresidencyMappingService, PresidencyMappingService>();
        services.AddScoped<IStateMappingService, StateMappingService>();
        services.AddScoped<IEntityMappingService, EntityMappingService>();
        services.AddScoped<ICantonMappingService, CantonMappingService>();
        services.AddScoped<IMunicipalityMappingService, MunicipalityMappingService>();

        services.AddScoped<IElectionYearsService, DbElectionYearsService>();
        services.AddScoped<IRankingService, RankingService>();
        services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddSingleton<RabbitMqPersistentConnection>();
        services.AddScoped<IUserInviteQueue, RabbitMqUserInviteQueue>();
        services.AddScoped<IPasswordResetQueue, RabbitMqPasswordResetQueue>();
        services.AddMemoryCache();
    }
}
