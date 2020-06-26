CREATE TABLE [dbo].[ArtistWorkStationOccurrence] (
    [StartTime]    DATETIMEOFFSET (7) NOT NULL,
    [StationId]    BIGINT             NOT NULL,
    [ArtistWorkId] BIGINT             NOT NULL,
    CONSTRAINT [PK_ArtistWorkStationOccurrence] PRIMARY KEY CLUSTERED ([StartTime] ASC, [StationId] ASC),
    CONSTRAINT [FK_ArtistWorkStationOccurrence_ArtistWork] FOREIGN KEY ([ArtistWorkId]) REFERENCES [dbo].[ArtistWork] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_ArtistWorkStationOccurrence_Station] FOREIGN KEY ([StationId]) REFERENCES [dbo].[Station] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

