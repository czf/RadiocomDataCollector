using NUnit.Framework;
using NSubstitute;
using Czf.Repository.Radiocom;
using Microsoft.Extensions.Options;
using static Czf.Repository.Radiocom.SqlRadiocomRepository;
using System.Data;
using System.Collections.Generic;
using System;
using Dapper;
using static Dapper.SqlMapper;
using Microsoft.SqlServer.Server;
using System.Threading.Tasks;
using System.Data.Common;

namespace Czf.Repository.Radiocom.Test
{
    public class RepositoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestOf(typeof(SqlRadiocomRepository))]
        public async Task ProcessRawOccurrancesTest()
        {
            #region arrange
            IOptionsSnapshot<SqlRadiocomRepositoryOptions> options = Substitute.For<IOptionsSnapshot<SqlRadiocomRepositoryOptions>>();
            options.Value.Returns(new SqlRadiocomRepositoryOptions() { ConnectionString = "connectionstring" });

            DbConnection dbConnection = Substitute.For<DbConnection>();
            DbCommand dbCommand = Substitute.For<DbCommand>();
            DbParameterCollection dataParameters = Substitute.For<DbParameterCollection>();
            dbConnection.CreateCommand().Returns(dbCommand);
            dbConnection.State.Returns(ConnectionState.Open);
            dbConnection.When(x => x.OpenAsync()).Do(x => dbConnection.State.Returns(ConnectionState.Open));
            dbConnection.When(x => x.CloseAsync()).Do(x => dbConnection.State.Returns(ConnectionState.Closed));
            dbCommand.Parameters.Returns(dataParameters);
            dbCommand.ExecuteNonQueryAsync().Returns(1);
            
            DbParameter dbDataParameter1 = Substitute.For<DbParameter>();
            DbParameter dbDataParameter2 = Substitute.For<DbParameter>();
            dbCommand.When(x => x.ExecuteNonQueryAsync()).Do(x => dbDataParameter2.Value = 1);
            dbCommand.CreateParameter().Returns(dbDataParameter1, dbDataParameter2);

            IDbConnectionFactory dbConnectionFactory = Substitute.For<IDbConnectionFactory>();
            dbConnectionFactory.GetConnection(Arg.Is(options.Value.ConnectionString)).Returns(dbConnection);

            SqlRadiocomRepository repo = new SqlRadiocomRepository(options, dbConnectionFactory);

            List<RawArtistWorkStationOccurrence> list = new List<RawArtistWorkStationOccurrence>();
            list.Add(new RawArtistWorkStationOccurrence() { Artist = "artist1", StartTime = DateTimeOffset.MinValue, StationId = 1, Title = "title1" });
            #endregion arrange

            #region act
            int output = await repo.ProcessRawOccurrances(list);
            #endregion act

            #region assert
            Assert.AreEqual("[dbo].[USP_ProcessRawArtistWorkStationOccurrences]", dbCommand.CommandText);
            Assert.AreEqual(CommandType.StoredProcedure, dbCommand.CommandType);
            Assert.AreEqual(1, output);
            Assert.AreEqual("IncomingOccurrences", dbDataParameter1.ParameterName);
            Assert.AreEqual(1, ((RawArtistWorkStationOccurrenceDataRecords)dbDataParameter1.Value).AsList().Count);
            Assert.AreEqual("NewOccurrenceInsertedCount", dbDataParameter2.ParameterName);
            Assert.AreEqual(ParameterDirection.Output, dbDataParameter2.Direction);
            Assert.AreEqual(DbType.Int32, dbDataParameter2.DbType);
            dbConnection.Received(1).Dispose();

            #endregion assert

        }

        [Test]
        [TestOf(typeof(RawArtistWorkStationOccurrenceDataRecords))]
        public void RawArtistWorkStationOccurrenceDataRecordsTest()
        {
            #region arrange
            
            List<RawArtistWorkStationOccurrence> list = new List<RawArtistWorkStationOccurrence>();
            list.Add(new RawArtistWorkStationOccurrence() { Artist = "artist1", StartTime = DateTimeOffset.MinValue, StationId = 1, Title = "title1" });
            list.Add(new RawArtistWorkStationOccurrence() { Artist = "artist2", StartTime = DateTimeOffset.MinValue.AddMinutes(5), StationId = 1, Title = "title2" });
                        
            RawArtistWorkStationOccurrenceDataRecords records = new RawArtistWorkStationOccurrenceDataRecords(list);
            #endregion arrange

            #region act
            List<SqlDataRecord>recordsList = records.AsList();
            #endregion act

            #region assert
            CollectionAssert.IsNotEmpty(recordsList);
            Assert.AreEqual(list[0].StartTime, recordsList[0].GetDateTimeOffset(0));
            Assert.AreEqual(list[0].StationId, recordsList[0].GetInt64(1));
            Assert.AreEqual(list[0].Artist, recordsList[0].GetString(2));
            Assert.AreEqual(list[0].Title, recordsList[0].GetString(3));

            Assert.AreEqual(list[1].StartTime, recordsList[1].GetDateTimeOffset(0));
            Assert.AreEqual(list[1].StationId, recordsList[1].GetInt64(1));
            Assert.AreEqual(list[1].Artist, recordsList[1].GetString(2));
            Assert.AreEqual(list[1].Title, recordsList[1].GetString(3));

            #endregion assert

        }
    }
}