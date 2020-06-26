CREATE TYPE [dbo].[RawArtistWorkStationOccurrenceTableType] AS TABLE
(
	StartTime DATETIMEOFFSET,
	StationId BIGINT,
	Artist NVARCHAR(100), 
	Title NVARCHAR(100)
)
