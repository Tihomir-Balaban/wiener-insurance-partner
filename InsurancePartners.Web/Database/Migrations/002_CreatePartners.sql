IF OBJECT_ID(N'dbo.Partners', N'U') IS NULL
BEGIN
CREATE TABLE dbo.Partners
(
    Id INT IDENTITY(1,1) NOT NULL,

    FirstName NVARCHAR(255) NOT NULL,
    LastName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(500) NULL,

    PartnerNumber CHAR(20) NOT NULL,
    CroatianPIN CHAR(11) NULL,

    PartnerTypeId INT NOT NULL,

    CreatedAtUtc DATETIME2(7) NOT NULL
            CONSTRAINT DF_Partners_CreatedAtUtc DEFAULT SYSUTCDATETIME(),

    CreateByUser NVARCHAR(255) NOT NULL,

    IsForeign BIT NOT NULL,

    ExternalCode NVARCHAR(20) NOT NULL,

    Gender CHAR(1) NOT NULL,

    UpdatedAtUtc DATETIME2(7) NULL,

    IsDeleted BIT NOT NULL
        CONSTRAINT DF_Partners_IsDeleted DEFAULT 0,

    DeletedAtUtc DATETIME2(7) NULL,

    RowVersion ROWVERSION NOT NULL,

    CONSTRAINT PK_Partners PRIMARY KEY (Id),

    CONSTRAINT FK_Partners_PartnerTypes
        FOREIGN KEY (PartnerTypeId)
            REFERENCES dbo.PartnerTypes (Id),

    CONSTRAINT CK_Partners_FirstName_Length
        CHECK (LEN(LTRIM(RTRIM(FirstName))) BETWEEN 2 AND 255),

    CONSTRAINT CK_Partners_LastName_Length
        CHECK (LEN(LTRIM(RTRIM(LastName))) BETWEEN 2 AND 255),

    CONSTRAINT CK_Partners_PartnerNumber_Format
        CHECK (
            LEN(PartnerNumber) = 20
                AND PartnerNumber NOT LIKE '%[^0-9]%'
            ),

    CONSTRAINT CK_Partners_CroatianPIN_Format
        CHECK (
            CroatianPIN IS NULL
                OR (
                LEN(CroatianPIN) = 11
                    AND CroatianPIN NOT LIKE '%[^0-9]%'
                )
            ),

    CONSTRAINT CK_Partners_CreateByUser_Length
        CHECK (LEN(LTRIM(RTRIM(CreateByUser))) BETWEEN 3 AND 255),

    CONSTRAINT CK_Partners_ExternalCode_Length
        CHECK (LEN(LTRIM(RTRIM(ExternalCode))) BETWEEN 10 AND 20),

    CONSTRAINT CK_Partners_Gender
        CHECK (Gender IN ('M', 'F', 'N')),

    CONSTRAINT CK_Partners_DeletedAtUtc
        CHECK (
            (IsDeleted = 0 AND DeletedAtUtc IS NULL)
                OR
            (IsDeleted = 1 AND DeletedAtUtc IS NOT NULL)
            )
);
END;