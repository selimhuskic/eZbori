using Microsoft.Data.SqlClient;

namespace DAL.Data.Connection
{
    public class ConnectionFactory(string connectionString) : IConnectionFactory
    {
        private readonly string _connectionString = connectionString;

        public SqlConnection SqlConnection() => new(ConnectionString());

        public virtual string ConnectionString() => _connectionString;
    }
}
