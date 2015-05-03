CREATE TABLE [dbo].[Account]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY,
	[Name] VARCHAR(100) NOT NULL,
	[IsActive] BIT NOT NULL DEFAULT 0,
	[PrimaryContactId] uniqueidentifier NOT NULL , 
    CONSTRAINT [FK_Account_Contact] FOREIGN KEY ([Id]) REFERENCES [dbo].[Contact]([Id])
)
