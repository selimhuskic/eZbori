using Application.Interfaces;
using DAL.Analysis.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace Boundary.DALConfig;

public static class StrategiesConfiguration
{
    public static void AddStrategies(this IServiceCollection services)
    {
        // Parties results strategies (strategy pattern — one per AnalysisSubject)
        services.AddScoped<IPartiesResultsStrategy, StateElectoralUnitPartiesStrategy>();
        services.AddScoped<IPartiesResultsStrategy, StateMunicipalPartiesStrategy>();
        services.AddScoped<IPartiesResultsStrategy, EntityElectoralUnitPartiesStrategy>();
        services.AddScoped<IPartiesResultsStrategy, EntityMunicipalPartiesStrategy>();
        services.AddScoped<IPartiesResultsStrategy, CantonElectoralUnitPartiesStrategy>();
        services.AddScoped<IPartiesResultsStrategy, CantonMunicipalPartiesStrategy>();
        services.AddScoped<IPartiesResultsStrategy, PresidencyResultsStrategy>();
        services.AddScoped<IPartiesResultsStrategy, MunicipalCouncilPartiesStrategy>();
        services.AddScoped<IPartiesResultsStrategy, MayorDetailsStrategy>();
    }   
}
