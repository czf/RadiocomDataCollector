using System.Data;
using System.Threading.Tasks;

namespace Czf.Repository.Radiocom
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> GetConnection(string connectionString);
    }
}

