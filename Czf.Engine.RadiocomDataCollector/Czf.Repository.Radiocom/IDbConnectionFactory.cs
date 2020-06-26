using System.Data;


namespace Czf.Repository.Radiocom
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection(string connectionString);
    }
}

