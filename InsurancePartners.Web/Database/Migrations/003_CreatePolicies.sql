IF OBJECT_ID(N'dbo.Policies', N'U') IS NULL
BEGIN
CREATE TABLE dbo.Policies
(
    Id INT IDENTITY(1,1) NOT NULL,

    PartnerId INT NOT NULL,

    PolicyNumber NVARCHAR(15) NOT NULL,

    PolicyAmount DECIMAL(18, 2) NOT NULL,

    CreatedAtUtc DATETIME2(7) NOT NULL
            CONSTRAINT DF_Policies_CreatedAtUtc DEFAULT SYSUTCDATETIME(),

    UpdatedAtUtc DATETIME2(7) NULL,

    IsDeleted BIT NOT NULL
        CONSTRAINT DF_Policies_IsDeleted DEFAULT 0,

    DeletedAtUtc DATETIME2(7) NULL,

    RowVersion ROWVERSION NOT NULL,

    CONSTRAINT PK_Policies PRIMARY KEY (Id),

    CONSTRAINT FK_Policies_Partners
        FOREIGN KEY (PartnerId)
            REFERENCES dbo.Partners (Id),

    CONSTRAINT CK_Policies_PolicyNumber_Length
        CHECK (LEN(LTRIM(RTRIM(PolicyNumber))) BETWEEN 10 AND 15),

    CONSTRAINT CK_Policies_PolicyAmount_Positive
        CHECK (PolicyAmount > 0),

    CONSTRAINT CK_Policies_DeletedAtUtc
        CHECK (
            (IsDeleted = 0 AND DeletedAtUtc IS NULL)
                OR
            (IsDeleted = 1 AND DeletedAtUtc IS NOT NULL)
            )
);
END;