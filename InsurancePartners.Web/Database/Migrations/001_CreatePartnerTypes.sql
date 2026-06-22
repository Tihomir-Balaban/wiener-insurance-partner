IF OBJECT_ID(N'dbo.PartnerTypes', N'U') IS NULL
BEGIN
CREATE TABLE dbo.PartnerTypes
(
    Id INT NOT NULL,
    Name NVARCHAR(50) NOT NULL,

    CONSTRAINT PK_PartnerTypes PRIMARY KEY (Id),
    CONSTRAINT UQ_PartnerTypes_Name UNIQUE (Name)
);
END;