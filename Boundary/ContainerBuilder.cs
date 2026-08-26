using Boundary.ApplicationServicesConfig;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Boundary.ExternalConfig;
using Microsoft.AspNetCore.Builder;
using DAL.Assembly;
using Boundary.DALConfig;

namespace Boundary;

public class ContainerBuilder
{
    private readonly IConfiguration _configuration;
    private readonly IServiceCollection _services;
    private IApplicationBuilder _appBuilder;

    public ContainerBuilder(IConfiguration configuration, IServiceCollection services)
    {
        _configuration = configuration;
        _services = services;
    }

    public ContainerBuilder(IConfiguration configuration)
    {
        _configuration = configuration;
    } 

    public static ContainerBuilder AContainerBuilder(IServiceCollection services, IConfiguration configuration)
    {
        return new ContainerBuilder(configuration, services);
    }

    public static ContainerBuilder BContainerBuilder(IConfiguration configuration, IServiceCollection services)
    {
        return new ContainerBuilder(configuration, services);
    }

    public static ContainerBuilder AContainerBuilder(IConfiguration configuration)
    {
        return new ContainerBuilder(configuration);
    }

    public ContainerBuilder WithApplication(IApplicationBuilder appBuilder)
    {
        _appBuilder = appBuilder;
        return this;
    }

    public ContainerBuilder WithSql(string connectionString)
    {
        _services.AddSql(connectionString);
        return this;
    }

    public ContainerBuilder WithStrategies()
    {
        _services.AddStrategies();
        return this;
    }

    public ContainerBuilder WithMediatR()
    {
        _services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DalAssembly).Assembly));
        return this;
    }

    public ContainerBuilder WithExternal()
    {
        _services.AddHttpClients();
        return this;
    }

    public ContainerBuilder WithApplicationServices()
    {
        _services.RegisterApplicationServices(_configuration);
        return this;
    }
}