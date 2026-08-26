using DbUp;
using DbUp.Engine;

namespace Migrations.SetUp;

public class DbUpgrader(DbUpgraderConfiguration configuration)
{
    private readonly DbUpgraderConfiguration _configuration = configuration;

    public void Execute(IEnumerable<SqlScript> sqlScripts)
    {
        var result = DeployChanges.To
            .SqlDatabase(_configuration.ConnectionString)
            .WithScripts(sqlScripts)
            .WithTransactionPerScript()
            .WithExecutionTimeout(_configuration.ExecutionTimeout)
            .JournalToSqlTable("dbo", _configuration.MigrationTable)
            .LogToConsole()
            .Build()
            .PerformUpgrade();

        VerifyDatabaseUpgrade(result);
    }

    private static void VerifyDatabaseUpgrade(DatabaseUpgradeResult databaseUpgradeResult)
    {
        if (databaseUpgradeResult.Successful) return;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Sql Upgrade not successful...");
        Console.WriteLine(databaseUpgradeResult.Error);
        Console.ResetColor();

        throw new InvalidOperationException($"Sql Upgrade not successful: {databaseUpgradeResult.Error}");
    }
}
