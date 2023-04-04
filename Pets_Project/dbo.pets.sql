USE [C:\USERS\PETYT\SOURCE\REPOS\PETS_PROJECT\PETS_PROJECT\PETS_DB.MDF]
GO

/****** Object: Table [dbo].[pets] Script Date: 4/4/2023 4:18:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[pets] (
    [pet_id]    INT            NOT NULL,
    [type_id]   INT            NOT NULL,
    [username]  NVARCHAR (15)  NOT NULL,
    [password]  NVARCHAR (25)  NOT NULL,
    [birthdate] DATE           NOT NULL,
    [pet_name]  NVARCHAR (30)  NOT NULL,
    [health]    NVARCHAR (250) NOT NULL
);


