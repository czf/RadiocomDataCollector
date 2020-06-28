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
                conn.AccessToken = await (new AzureServiceTokenProvider( connectionString: "RunAs=App;AppId=0938c8ae-211d-43ee-a8c6-878f08f42471")).GetAccessTokenAsync("https://database.windows.net/", "4e72a007-c1d8-4b0f-8fb6-5137abe83221");
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

