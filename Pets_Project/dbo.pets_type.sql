USE [C:\USERS\PETYT\SOURCE\REPOS\PETS_PROJECT\PETS_PROJECT\PETS_DB.MDF]
GO

/****** Object: Table [dbo].[pets_type] Script Date: 4/4/2023 4:19:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[pets_type] (
    [type_id]   INT           NOT NULL,
    [type_name] NVARCHAR (10) NOT NULL
);


