CREATE TABLE [dbo].[pets_type] (
    [type_id]   INT           NOT NULL,
    [type_name] NVARCHAR (10) NOT NULL,
	CONSTRAINT [PK_pets_type] PRIMARY KEY CLUSTERED ([type_id] ASC)
);

