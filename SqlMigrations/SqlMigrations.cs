using System.Reflection;
using System.Text;
using DbUp;
using DbUp.Engine;
using Migrations.SetUp;

namespace Migrations;

public class SqlMigrations
{
    public static void Run(string? connectionString = null, bool shouldCreateDatabaseIfNotExist = true)
    {
        var sqlConnectionString = connectionString
            ?? throw new InvalidOperationException("A connection string must be provided.");

        if (shouldCreateDatabaseIfNotExist)
        {
            EnsureDatabase.For.SqlDatabase(sqlConnectionString);
        }

        CreateDbUpgrader(sqlConnectionString).Execute(SqlScripts);
    }

    private static DbUpgrader CreateDbUpgrader(string connectionString)
    {
        var dbUpgraderConfig = new DbUpgraderConfiguration
        {
            ConnectionString = connectionString,
            MigrationTable = "_SchemaVersions",
            ExecutionTimeout = TimeSpan.FromMinutes(1)
        };

        return new DbUpgrader(dbUpgraderConfig);
    }

    private static IEnumerable<SqlScript> SqlScripts
    {
        get
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetAssembly(typeof(SqlMigrations))?.Location) ?? throw new SystemException("No path provided for SQL scripts!");
            return GetSqlScriptsFromDirectory(Path.Combine(currentDirectory, "Scripts")).ToList();
        }
    }

    private static IEnumerable<SqlScript> GetSqlScriptsFromDirectory(string scriptsDirectory)
    {
        if (!Directory.Exists(scriptsDirectory))
        {
            yield break;
        }

        var files = Directory.GetFiles(scriptsDirectory).OrderBy(fileName => fileName);

        foreach (var file in files)
        {
            yield return SqlScript.FromFile(file, Encoding.UTF8);
        }
    }
}