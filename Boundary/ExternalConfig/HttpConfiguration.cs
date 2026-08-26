using External.CentralElectionCommiteeHttpClients;
using Microsoft.Extensions.DependencyInjection;

namespace Boundary.ExternalConfig
{
    public static class HttpConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<IPresidencyClient, PresidencyClient>();
            services.AddHttpClient<IStateClient, StateClient>();
            services.AddHttpClient<IEntityClient, EntityClient>();
            services.AddHttpClient<ICantonClient, CantonClient>();
            services.AddHttpClient<ILocalElectionsClient, LocalElectionsClient>();
        }
    }
}
