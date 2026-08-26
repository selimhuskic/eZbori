using Microsoft.Extensions.Configuration;

namespace Migrations;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        var connectionString = configuration["ConnectionStrings:DefaultConnection"]
            ?? throw new InvalidOperationException(
                "Connection string not found. Set the 'ConnectionStrings__DefaultConnection' environment variable or pass --ConnectionStrings:DefaultConnection=<value>.");

        SqlMigrations.Run(connectionString);

        Console.WriteLine("Database migration completed successfully.");
    }
}
