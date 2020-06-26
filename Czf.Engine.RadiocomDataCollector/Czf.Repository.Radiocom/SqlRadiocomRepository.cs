using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;


namespace Czf.Repository.Radiocom
{
    public class SqlRadiocomRepository : IRadiocomRepository
    {
        private readonly SqlRadiocomRepositoryOptions _sqlRadiocomRepositoryOptions;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlRadiocomRepository(IOptions<SqlRadiocomRepositoryOptions> sqlRadiocomRepositoryOptions, IDbConnectionFactory dbConnectionFactory)
        {
            _sqlRadiocomRepositoryOptions = sqlRadiocomRepositoryOptions.Value;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<int> ProcessRawOccurrances(IEnumerable<RawArtistWorkStationOccurrence> occurrences)
        {
            int inserted = -1;
            using (IDbConnection conn = _dbConnectionFactory.GetConnection(_sqlRadiocomRepositoryOptions.ConnectionString))
            {
                RawArtistWorkStationOccurrenceDataRecords records = new RawArtistWorkStationOccurrenceDataRecords(occurrences);

                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@IncomingOccurrences", records.AsTableValuedParameter("dbo.RawArtistWorkStationOccurrenceTableType"));
                dynamicParameters.Add("@NewOccurrenceInsertedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var affected = await conn.ExecuteAsync("[dbo].[USP_ProcessRawArtistWorkStationOccurrences]", dynamicParameters, commandType: CommandType.StoredProcedure);
                inserted = dynamicParameters.Get<int>("@NewOccurrenceInsertedCount");
            }
            return inserted;
        }

        public class SqlRadiocomRepositoryOptions
        {
            public const string SqlRadiocomRepository = "SqlRadiocomRepositoryOptions";
            public string ConnectionString { get; set; }
        }



    }
}

