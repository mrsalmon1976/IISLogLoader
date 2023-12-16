IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA + '.' + TABLE_NAME = '{{TableName}}')
BEGIN
    CREATE TABLE {{TableName}} (
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[FilePath] [nvarchar](256) NOT NULL,
		[DateTime] datetime2 NOT NULL,
		[ClientIpAddress] [nvarchar](50) NULL,
		[BytesReceived] [bigint] NULL,
		[Cookie] [nvarchar](max) NULL,
		[Host] [nvarchar](250) NULL,
		[Method] [nvarchar](20) NULL,
		[Referrer] [nvarchar](max) NULL,
		[UriQuery] [nvarchar](max) NULL,
		[UriStem] [nvarchar](max) NULL,
		[UserAgent] [nvarchar](max) NULL,
		[UserName] [nvarchar](256) NULL,
		[ProtocolVersion] [nvarchar](50) NULL,
		[ServerName] [nvarchar](250) NULL,
		[ServerIpAddress] [nvarchar](50) NULL,
		[ServerPort] [int] NULL,
		[SiteName] [nvarchar](250) NULL,
		[BytesSent] [bigint] NULL,
		[StatusCode] [nvarchar](50) NULL,
		[ProtocolSubstatus] [nvarchar](100) NULL,
		[WindowsStatusCode] [nvarchar](100) NULL,
		[TimeTaken] [int] NULL
	 CONSTRAINT [PK_{{TableName}}] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] 
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_{{IndexName}}_FilePath' AND object_id = OBJECT_ID('{{TableName}}'))
BEGIN
	CREATE INDEX IX_{{IndexName}}_FilePath ON {{TableName}} (FilePath);
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_{{IndexName}}_DateTime' AND object_id = OBJECT_ID('{{TableName}}'))
BEGIN
	CREATE INDEX IX_{{IndexName}}_DateTime ON {{TableName}} ([DateTime]);
END
