if exists 
(
    select so1.id from syscolumns so1 
    join sysobjects so2 on so1.id=so2.id
    where so1.name = 'Description' and so2.Name = 'Treatments'
)
begin
    alter table [dbo].[Treatments] alter column [Description] nvarchar(500) NOT NULL
end
go
