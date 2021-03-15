using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.ApiWrapper.Radiocom;
using Czf.Repository.Radiocom;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NUnit.Framework;

namespace Czf.Engine.RadiocomDataCollector.Test
{
    public class EngineTests
    {
        private ILogger<RadiocomDataCollectorEngine> log;
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            log = Substitute.For<ILogger<RadiocomDataCollectorEngine>>();
        }


        [Test]
        [TestOf(typeof(RadiocomExtentions))]
        public void ToRawOccurrencesTest()
        {
            #region arrange
            StationRecentlyPlayedResponse response = GetStationRecentlyPlayedResponse();
            #endregion arrange
            #region act
            List<RawArtistWorkStationOccurrence> result = response.ToRawOccurrences().ToList();
            #endregion act
            #region assert
            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(response.Schedule[3].Artist, result[3].Artist);
            Assert.AreEqual(response.Schedule[0].Title, result[0].Title);
            Assert.AreEqual(response.Schedule[1].StartTime, result[1].StartTime);
            Assert.AreEqual(response.Station.Id, result[2].StationId);
            #endregion assert
        }

        [Test]
        [TestOf(typeof(RadiocomDataCollectorEngine))]
        public async Task Run_RawOccurrancesProcessedUntilHoursBackToRetrieve()
        {
            #region arrange
            IDateTimeOffsetProvider dateTimeOffsetProvider = Substitute.For<IDateTimeOffsetProvider>();
            DateTimeOffset now = DateTimeOffset.MaxValue.Subtract(TimeSpan.FromSeconds(86399));
            dateTimeOffsetProvider.UtcNow.Returns(now);

            IRadiocomClient client = Substitute.For<IRadiocomClient>();
            IRadiocomRepository repository = Substitute.For<IRadiocomRepository>();
            IOptions<RadiocomDataCollectorEngineOptions> optionsSnapshot = Substitute.For<IOptions<RadiocomDataCollectorEngineOptions>>();
            RadiocomDataCollectorEngineOptions options = new RadiocomDataCollectorEngineOptions()
            {
                HoursBackToRetrive = 5
            };
            optionsSnapshot.Value.Returns(options);

            Task<StationRecentlyPlayedResponse> response = Task.FromResult(GetStationRecentlyPlayedResponse());
            client
                .StationRecentlyPlayed(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<ApiWrapper.Radiocom.DayOfWeek>())
                .Returns(response,
                Task.FromResult(GetStationRecentlyPlayedResponse(10)),
                Task.FromResult(GetStationRecentlyPlayedResponse(20)),
                Task.FromResult(GetStationRecentlyPlayedResponse(30)),
                Task.FromResult(GetStationRecentlyPlayedResponse(40))

                );
            
            repository.ProcessRawOccurrances(Arg.Any<IEnumerable<RawArtistWorkStationOccurrence>>())
                .Returns(response.Result.Schedule.Count);
            RadiocomDataCollectorEngine engine = new RadiocomDataCollectorEngine(client, repository, optionsSnapshot, dateTimeOffsetProvider, log);
            
            #endregion arrange
            #region act
            await engine.Run();
            #endregion act
            #region assert
            List<ICall> clientCalls = client.ReceivedCalls().AsList();
            
            
            await repository.Received(5).ProcessRawOccurrances(Arg.Any<IEnumerable<RawArtistWorkStationOccurrence>>());
            Assert.AreEqual(902, (int)clientCalls[0].GetArguments()[0]);
            Assert.AreEqual(902, (int)clientCalls[1].GetArguments()[0]);
            Assert.AreEqual(902, (int)clientCalls[2].GetArguments()[0]);
            Assert.AreEqual(902, (int)clientCalls[3].GetArguments()[0]);
            Assert.AreEqual(902, (int)clientCalls[4].GetArguments()[0]);

            Assert.AreEqual(now.Hour, (int)clientCalls[0].GetArguments()[1]);
            Assert.AreEqual(now.AddHours(-1).Hour, (int)clientCalls[1].GetArguments()[1]);
            Assert.AreEqual(now.AddHours(-2).Hour, (int)clientCalls[2].GetArguments()[1]);
            Assert.AreEqual(now.AddHours(-3).Hour, (int)clientCalls[3].GetArguments()[1]);
            Assert.AreEqual(now.AddHours(-4).Hour, (int)clientCalls[4].GetArguments()[1]);

            now = ConvertTime(now);

            Assert.AreEqual(now.DayOfWeek.ToString() , clientCalls[0].GetArguments()[2].ToString());
            Assert.AreEqual(now.DayOfWeek.ToString() , clientCalls[1].GetArguments()[2].ToString());
            Assert.AreEqual(now.DayOfWeek.ToString() , clientCalls[2].GetArguments()[2].ToString());
            Assert.AreEqual(now.DayOfWeek.ToString() , clientCalls[3].GetArguments()[2].ToString());
            Assert.AreEqual(now.DayOfWeek.ToString() , clientCalls[4].GetArguments()[2].ToString());
            
            #endregion assert
        }



        [Test]
        [TestOf(typeof(RadiocomDataCollectorEngine))]
        public async Task Run_RawOccurrancesProcessedOnceForNonUniqueResponseScheduleUntilHoursBackToRetrieve()
        {
            #region arrange
            IDateTimeOffsetProvider dateTimeOffsetProvider = Substitute.For<IDateTimeOffsetProvider>();
            DateTimeOffset now = DateTimeOffset.MaxValue.Subtract(TimeSpan.FromSeconds(86399));
            dateTimeOffsetProvider.UtcNow.Returns(now);

            IRadiocomClient client = Substitute.For<IRadiocomClient>();
            IRadiocomRepository repository = Substitute.For<IRadiocomRepository>();
            IOptions<RadiocomDataCollectorEngineOptions> optionsSnapshot = Substitute.For<IOptions<RadiocomDataCollectorEngineOptions>>();
            RadiocomDataCollectorEngineOptions options = new RadiocomDataCollectorEngineOptions()
            {
                HoursBackToRetrive = 5
            };
            optionsSnapshot.Value.Returns(options);

            
            Task<StationRecentlyPlayedResponse> response = Task.FromResult(GetStationRecentlyPlayedResponse());
            client
                .StationRecentlyPlayed(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<ApiWrapper.Radiocom.DayOfWeek>())
                .Returns(response);

            repository.ProcessRawOccurrances(Arg.Any<IEnumerable<RawArtistWorkStationOccurrence>>())
                .Returns(response.Result.Schedule.Count, 0);
            RadiocomDataCollectorEngine engine = new RadiocomDataCollectorEngine(client, repository, optionsSnapshot, dateTimeOffsetProvider, log);

            #endregion arrange
            #region act
            await engine.Run();
            #endregion act
            #region assert
            List<ICall> clientCalls = client.ReceivedCalls().AsList();


            await repository.Received(1).ProcessRawOccurrances(Arg.Any<IEnumerable<RawArtistWorkStationOccurrence>>());
            Assert.AreEqual(902, (int)clientCalls[0].GetArguments()[0]);
            Assert.AreEqual(902, (int)clientCalls[1].GetArguments()[0]);
            Assert.AreEqual(902, (int)clientCalls[2].GetArguments()[0]);
            Assert.AreEqual(902, (int)clientCalls[3].GetArguments()[0]);
            Assert.AreEqual(902, (int)clientCalls[4].GetArguments()[0]);

            Assert.AreEqual(now.Hour, (int)clientCalls[0].GetArguments()[1]);
            Assert.AreEqual(now.AddHours(-1).Hour, (int)clientCalls[1].GetArguments()[1]);
            Assert.AreEqual(now.AddHours(-2).Hour, (int)clientCalls[2].GetArguments()[1]);
            Assert.AreEqual(now.AddHours(-3).Hour, (int)clientCalls[3].GetArguments()[1]);
            Assert.AreEqual(now.AddHours(-4).Hour, (int)clientCalls[4].GetArguments()[1]);

            now = ConvertTime(now);
            Assert.AreEqual(now.DayOfWeek.ToRadiocomDayOfWeek().ToString(), clientCalls[0].GetArguments()[2].ToString());
            
            
            Assert.AreEqual(now.DayOfWeek.ToString(), clientCalls[1].GetArguments()[2].ToString());
            Assert.AreEqual(now.DayOfWeek.ToString(), clientCalls[2].GetArguments()[2].ToString());
            Assert.AreEqual(now.DayOfWeek.ToString(), clientCalls[3].GetArguments()[2].ToString());
            Assert.AreEqual(now.DayOfWeek.ToString(), clientCalls[4].GetArguments()[2].ToString());

            #endregion assert
        }

        [Test]
        [TestOf(typeof(RadiocomDataCollectorEngine))]
        public async Task Run_RawOccurrancesProcessedOnceForNonUniqueResponseScheduleUntilOverlapScheduleResposne()
        {
            #region arrange
            IDateTimeOffsetProvider dateTimeOffsetProvider = Substitute.For<IDateTimeOffsetProvider>();
            DateTimeOffset now = DateTimeOffset.MaxValue.Subtract(TimeSpan.FromSeconds(86399));
            dateTimeOffsetProvider.UtcNow.Returns(now);

            IRadiocomClient client = Substitute.For<IRadiocomClient>();
            IRadiocomRepository repository = Substitute.For<IRadiocomRepository>();
            IOptions<RadiocomDataCollectorEngineOptions> optionsSnapshot = Substitute.For<IOptions<RadiocomDataCollectorEngineOptions>>();
            RadiocomDataCollectorEngineOptions options = new RadiocomDataCollectorEngineOptions()
            {
                HoursBackToRetrive = 5
            };
            optionsSnapshot.Value.Returns(options);
            
            
            var responseData1 = GetStationRecentlyPlayedResponse(10);
            var responseData2 = GetStationRecentlyPlayedResponse(10);
            responseData2.Schedule = responseData2.Schedule.Skip(1).Union(GetStationRecentlyPlayedResponse(20).Schedule.Take(1)).ToList();

            Task <StationRecentlyPlayedResponse> responseTask1 = Task.FromResult(responseData1);
            Task <StationRecentlyPlayedResponse> responseTask2 = Task.FromResult(responseData2);
            client
                .StationRecentlyPlayed(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<ApiWrapper.Radiocom.DayOfWeek>())
                .Returns(responseTask1, responseTask2);

            repository.ProcessRawOccurrances(Arg.Any<IEnumerable<RawArtistWorkStationOccurrence>>())
                .Returns(responseTask1.Result.Schedule.Count, 0);
            RadiocomDataCollectorEngine engine = new RadiocomDataCollectorEngine(client, repository, optionsSnapshot, dateTimeOffsetProvider, log);

            #endregion arrange
            #region act
            await engine.Run();
            #endregion act
            #region assert
            List<ICall> clientCalls = client.ReceivedCalls().AsList();
            List<ICall> processCalls = repository.ReceivedCalls().AsList();

            await repository.Received(2).ProcessRawOccurrances(Arg.Any<IEnumerable<RawArtistWorkStationOccurrence>>());

            Assert.AreEqual(responseData1.Schedule.Count,
                ((IEnumerable<RawArtistWorkStationOccurrence>)processCalls[0].GetArguments()[0]).Count());
            Assert.AreEqual(1,
                ((IEnumerable<RawArtistWorkStationOccurrence>)processCalls[1].GetArguments()[0]).Count());


            Assert.AreEqual(902, (int)clientCalls[0].GetArguments()[0]);
            Assert.AreEqual(902, (int)clientCalls[1].GetArguments()[0]);
           

            Assert.AreEqual(now.Hour, (int)clientCalls[0].GetArguments()[1]);
            Assert.AreEqual(now.AddHours(-1).Hour, (int)clientCalls[1].GetArguments()[1]);

            now = ConvertTime(now);
            Assert.AreEqual(now.DayOfWeek.ToString(), clientCalls[0].GetArguments()[2].ToString());
            Assert.AreEqual(now.DayOfWeek.ToString(), clientCalls[1].GetArguments()[2].ToString());
           

            #endregion assert
        }


        [Test]
        [TestOf(typeof(RadiocomDataCollectorEngine))]
        public async Task Run_RunOnceNoRawOccurrancesProcessedAsync()
        {


            #region arrange
            IDateTimeOffsetProvider dateTimeOffsetProvider = Substitute.For<IDateTimeOffsetProvider>();
            DateTimeOffset now = DateTimeOffset.MaxValue.Subtract(TimeSpan.FromSeconds(86399));
            dateTimeOffsetProvider.UtcNow.Returns(now);

            IRadiocomClient client = Substitute.For<IRadiocomClient>();
            IRadiocomRepository repository = Substitute.For<IRadiocomRepository>();
            IOptions<RadiocomDataCollectorEngineOptions> optionsSnapshot = Substitute.For<IOptions<RadiocomDataCollectorEngineOptions>>();
            RadiocomDataCollectorEngineOptions options = new RadiocomDataCollectorEngineOptions()
            {
                HoursBackToRetrive = 5
            };
            optionsSnapshot.Value.Returns(options);


            Task<StationRecentlyPlayedResponse> response = Task.FromResult(GetStationRecentlyPlayedResponse());
            client
                .StationRecentlyPlayed(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<ApiWrapper.Radiocom.DayOfWeek>())
                .Returns(response,
                Task.FromResult(GetStationRecentlyPlayedResponse(20)),
                Task.FromResult(GetStationRecentlyPlayedResponse(30)),
                Task.FromResult(GetStationRecentlyPlayedResponse(40)),
                Task.FromResult(GetStationRecentlyPlayedResponse(50)));

            repository.ProcessRawOccurrances(Arg.Any<IEnumerable<RawArtistWorkStationOccurrence>>())
                .Returns(0);
            RadiocomDataCollectorEngine engine = new RadiocomDataCollectorEngine(client, repository, optionsSnapshot, dateTimeOffsetProvider, log);

            #endregion arrange
            #region act
            await engine.Run();
            #endregion act
            #region assert
            List<ICall> clientCalls = client.ReceivedCalls().AsList();


            await repository.Received(1).ProcessRawOccurrances(Arg.Any<IEnumerable<RawArtistWorkStationOccurrence>>());
            Assert.AreEqual(902, (int)clientCalls[0].GetArguments()[0]);
            
            Assert.AreEqual(now.Hour, (int)clientCalls[0].GetArguments()[1]);

            now = ConvertTime(now);
            Assert.AreEqual(now.DayOfWeek.ToString(), clientCalls[0].GetArguments()[2].ToString());
            
            #endregion assert
        }

        private StationRecentlyPlayedResponse GetStationRecentlyPlayedResponse(int multiplier = 1) =>
            new StationRecentlyPlayedResponse()
            {
                Schedule = new List<ScheduleItem>()
                {
                    {new ScheduleItem(){Artist = "a0", StartTime=DateTimeOffset.MaxValue.AddMinutes(-3*multiplier), Title = "a1-1"} },
                    {new ScheduleItem(){Artist = "a1", StartTime=DateTimeOffset.MaxValue.AddMinutes(-7 * multiplier), Title = "a1-1"} },
                    {new ScheduleItem(){Artist = "a2", StartTime=DateTimeOffset.MaxValue.AddMinutes(-11*multiplier), Title = "a2-1"} },
                    {new ScheduleItem(){Artist = "a3", StartTime=DateTimeOffset.MaxValue.AddMinutes(-13 *multiplier), Title = "a3-1"} },
                },
                Station = new Station()
                {
                    Id = 404
                }
            };

        private DateTimeOffset ConvertTime(DateTimeOffset dateTime) =>  TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
    }
}


//#region arrange
//#endregion arrange
//#region act
//#endregion act
//#region assert
//#endregion assert