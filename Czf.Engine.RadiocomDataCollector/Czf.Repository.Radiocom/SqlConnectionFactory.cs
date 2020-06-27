using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;

namespace Czf.Repository.Radiocom
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private IOptionsMonitor<SqlConnectionFactoryOptions> _factoryOptions;
        public SqlConnectionFactory(IOptionsMonitor<SqlConnectionFactoryOptions> factoryOptions)
        {
            _factoryOptions = factoryOptions;
        }

        public async Task<IDbConnection> GetConnection(string connectionString)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            if (_factoryOptions.CurrentValue.SetAzureAdAccessToken)
            {
                conn.AccessToken = await (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/");
            }
            return conn;
        }

        public class SqlConnectionFactoryOptions
        {
            public const string SqlConnectionFactory = "SqlConnectionFactoryOptions";
            public bool SetAzureAdAccessToken { get; set; }
        }
    }
}

