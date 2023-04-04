USE [D:\UNI\3\3.2\USP\Pets_Project-master\Pets_Project\Database\PetsDB.mdf]
GO

/****** Object: Table [dbo].[vaccinations] Script Date: 4/4/2023 4:19:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[vaccinations] (
    [vacc_id]   INT            NOT NULL,
    [vacc_name] NVARCHAR (50)  NOT NULL,
    [vacc_desc] NVARCHAR (250) NOT NULL,
    [type_id]   INT            NOT NULL
);


