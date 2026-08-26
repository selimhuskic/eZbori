using Boundary;

namespace eZbori.Web.AppStart;

public static class ApiContainerBuilder
{
    public static void BootstrapApi(this IServiceCollection services, IConfiguration configuration)
    {
        ContainerBuilder.AContainerBuilder(services, configuration)
            .WithMediatR()
            .WithSql(configuration.GetConnectionString("DefaultConnection"))
            .WithExternal()
            .WithApplicationServices()
            .WithStrategies();
    }
}
