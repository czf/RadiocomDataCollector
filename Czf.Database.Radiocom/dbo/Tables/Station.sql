CREATE TABLE [dbo].[Station]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Category] NVARCHAR(50) NOT NULL, 
    [GmtOffset] SMALLINT NOT NULL, 
    [PlayingClass] NVARCHAR(50) NOT NULL 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Station Id from radio.com',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Station',
    @level2type = N'COLUMN',
    @level2name = N'Id'