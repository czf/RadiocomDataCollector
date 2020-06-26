CREATE TABLE [dbo].[ArtistWork] (
    [Id]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [ArtistId] BIGINT         NOT NULL,
    [Title]    NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ArtistWork_Artist] FOREIGN KEY ([ArtistId]) REFERENCES [dbo].[Artist] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);



GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'In most cases a song',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ArtistWork',
    @level2type = NULL,
    @level2name = NULL