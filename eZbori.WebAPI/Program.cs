using Boundary;
using Migrations;

namespace eZbori.Web;

public static class Program
{
    // Do not delete! 
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args)
            .Build();

        var configuration = host.Services.GetService(typeof(IConfiguration)) as IConfiguration;

        try
        {
            SqlMigrations.Run(configuration.GetConnectionString("DefaultConnection"));
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"[FATAL] Database migration failed: {ex.Message}");
            Console.ResetColor();
            throw;
        }

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }).ConfigureServices((hostBuilder, services) =>
            {
                ContainerBuilder.BContainerBuilder(hostBuilder.Configuration, services);
            });       
}