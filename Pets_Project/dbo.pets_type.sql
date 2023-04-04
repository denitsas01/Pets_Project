USE [D:\UNI\3\3.2\USP\Pets_Project-master\Pets_Project\Database\PetsDB.mdf]
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


