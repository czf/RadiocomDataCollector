using System.Data;
using System.Data.SqlClient;


namespace Czf.Repository.Radiocom
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetConnection(string connectionString) => new SqlConnection(connectionString);
    }
}

