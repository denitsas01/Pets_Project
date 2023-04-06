﻿CREATE TABLE [dbo].[pets] (
    [pet_id]    INT            NOT NULL IDENTITY,
    [type_id]   INT            NOT NULL,
    [username]  NVARCHAR (15)  NOT NULL,
    [password]  NVARCHAR (25)  NOT NULL,
    [birthdate] DATE           NOT NULL,
    [pet_name]  NVARCHAR (30)  NOT NULL,
    [health]    NVARCHAR (250) DEFAULT NULL,
    CONSTRAINT [PK_pets] PRIMARY KEY CLUSTERED ([pet_id] ASC),
    CONSTRAINT [FK_pets_pets_type] FOREIGN KEY ([type_id]) REFERENCES [dbo].[pets_type] ([type_id])
);

