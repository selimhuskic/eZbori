namespace Migrations.SetUp
{
    public class DbUpgraderConfiguration
    {
        public string MigrationTable { get; set; }
        public TimeSpan ExecutionTimeout { get; set; }
        public string ConnectionString { get; set; }
    }
}
