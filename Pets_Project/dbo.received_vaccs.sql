USE [D:\UNI\3\3.2\USP\Pets_Project-master\Pets_Project\Database\PetsDB.mdf]
GO

/****** Object: Table [dbo].[received_vaccs] Script Date: 4/4/2023 4:19:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[received_vaccs] (
    [rec_id]     INT NOT NULL,
    [vacc_id]    INT NOT NULL,
    [pet_id]     INT NOT NULL,
    [isReceived] BIT NOT NULL
);


