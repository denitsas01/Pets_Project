CREATE TABLE [dbo].[vaccinations] (
    [vacc_id]   INT            NOT NULL,
    [vacc_name] NVARCHAR (50)  NOT NULL,
    [vacc_desc] NVARCHAR (1000) NOT NULL,
    [type_id]   INT            NOT NULL,
    CONSTRAINT [PK_vaccinations] PRIMARY KEY CLUSTERED ([vacc_id] ASC),
    CONSTRAINT [FK_vaccinations_pets_type] FOREIGN KEY ([type_id]) REFERENCES [dbo].[pets_type] ([type_id])
);

