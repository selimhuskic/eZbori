using Microsoft.Data.SqlClient;

namespace DAL.Data.Connection
{
    public interface IConnectionFactory
    {
        SqlConnection SqlConnection();
    }
}
