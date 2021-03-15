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
            List<SqlDataRecord> recordsList = records.AsList();
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

        public static int[][] update_board(int[][] board)
        {
            int[][] newBoard = new int[board.Length][];
            for (int a = 0; a < board.Length; a++)
            {
                newBoard[a] = new int[board[a].Length];
                for (int b = 0; b < board[a].Length; b++)
                {
                    int countLive = 0;
                    if (a - 1 > 0 && b - 1 > 0 && board[a - 1][b - 1] == 1)
                    {
                        countLive++;
                    }
                    Console.WriteLine($"1: {a},{b}: {countLive}");

                    if (b - 1 > 0 && board[a][b - 1] == 1)
                    {
                        countLive++;
                    }
                    Console.WriteLine($"2: {a},{b}: {countLive}");
                    if (a + 1 < board.Length && b - 1 > 0 && board[a + 1][b - 1] == 1)
                    {
                        countLive++;
                    }
                    Console.WriteLine($"3: {a},{b}: {countLive}");
                    if (a - 1 > 0 && b + 1 < board[a].Length && board[a - 1][b + 1] == 1)
                    {
                        countLive++;
                    }

                    Console.WriteLine($"4: {a},{b}: {countLive}");
                    if (a + 1 < board.Length && b + 1 < board[a].Length && board[a + 1][b + 1] == 1)
                    {
                        countLive++;
                    }//here
                    Console.WriteLine($"5:  {a},{b}: {countLive}");
                    if (a + 1 < board.Length && board[a + 1][b] == 1)
                    {
                        countLive++;
                    }
                    Console.WriteLine($"6:  {a},{b}: {countLive}");
                    if (b + 1 < board[a].Length && board[a][b + 1] == 1)
                    {
                        countLive++;
                    }//here
                    Console.WriteLine($"7: {a},{b}: {countLive}");
                    if (a - 1 > 0 && board[a - 1][b] == 1)
                    {
                        countLive++;
                    }
                    Console.WriteLine($"8:  {a},{b}: {countLive}");
                    bool isAlive = board[a][b] == 1;
                    if (isAlive)
                    {
                        if (countLive == 2 || countLive == 3)
                        {
                            newBoard[a][b] = 1;
                        }
                        else
                        {
                            newBoard[a][b] = 0;
                        }
                    }
                    else if (countLive == 3)
                    {
                        newBoard[a][b] = 1;
                    }
                    else
                        newBoard[a][b] = 0;

                }
            }

            return newBoard;
        }
        [Test]
        public void test()
        {
            int[][] board = new int[4][];
            board[0] = new int[3] { 0, 1, 0 };
            board[1] = new int[3] { 0, 0, 1 };
            board[2] = new int[3] { 1, 1, 1 };
            board[3] = new int[3] { 0, 0, 0 };
            update_board(board);
        }
    }
    
}