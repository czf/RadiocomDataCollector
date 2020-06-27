CREATE PROCEDURE [dbo].[USP_ProcessRawArtistWorkStationOccurrences]
	@IncomingOccurrences dbo.RawArtistWorkStationOccurrenceTableType READONLY,
	@NewOccurrenceInsertedCount INT OUTPUT
	
AS
SET XACT_ABORT, NOCOUNT ON;

BEGIN TRY;
	BEGIN TRANSACTION;
	SET @NewOccurrenceInsertedCount = 0;
	INSERT INTO dbo.Artist
	([Name])
	SELECT 
	iop.Artist
	FROM @IncomingOccurrences iop
	WHERE NOT EXISTS (
		SELECT 1 FROM dbo.Artist a2
		WHERE a2.[Name] = iop.Artist);

	INSERT INTO dbo.ArtistWork
	(
		ArtistId,
		Title
	)
	SELECT
		a.Id,
		iop.Title
	FROM @IncomingOccurrences iop
	INNER JOIN dbo.Artist a ON iop.Artist = a.[Name]
	WHERE NOT EXISTS (
		SELECT 1 FROM dbo.ArtistWork aw2
		WHERE aw2.Title = iop.Title
		AND aw2.ArtistId = a.Id
	);

	INSERT INTO dbo.ArtistWorkStationOccurrence
	(
		StartTime,
		StationId,
		ArtistWorkId
	)
	SELECT 
		iop.StartTime,
		iop.StationId,
		aw.Id
	FROM @IncomingOccurrences iop
	INNER JOIN dbo.Artist a ON iop.Artist = a.[Name]
	INNER JOIN	dbo.ArtistWork aw ON aw.ArtistId = a.Id AND aw.Title = iop.Title
	WHERE NOT EXISTS(
		SELECT 1 FROM dbo.ArtistWorkStationOccurrence awso2 
		WHERE awso2.StartTime = iop.StartTime AND awso2.StationId = awso2.StationId);

	SET @NewOccurrenceInsertedCount = @@ROWCOUNT;
	COMMIT TRANSACTION;
END TRY
BEGIN CATCH;
	SET @NewOccurrenceInsertedCount = -1;
	THROW;

END CATCH
GO
GRANT EXECUTE
    ON OBJECT::[dbo].[USP_ProcessRawArtistWorkStationOccurrences] TO [RadioComCollectorDbAccount]
    AS [dbo];

