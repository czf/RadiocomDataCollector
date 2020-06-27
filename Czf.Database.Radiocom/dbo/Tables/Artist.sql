CREATE TABLE [dbo].[Artist] (
    [Id]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO
GRANT SELECT
    ON OBJECT::[dbo].[Artist] TO [RadioComCollectorDbAccount]
    AS [dbo];

