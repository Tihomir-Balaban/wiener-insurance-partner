IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Partners_PartnerNumber'
      AND object_id = OBJECT_ID(N'dbo.Partners')
)
BEGIN
CREATE UNIQUE INDEX UX_Partners_PartnerNumber
    ON dbo.Partners (PartnerNumber);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Partners_ExternalCode'
      AND object_id = OBJECT_ID(N'dbo.Partners')
)
BEGIN
CREATE UNIQUE INDEX UX_Partners_ExternalCode
    ON dbo.Partners (ExternalCode);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Partners_CroatianPIN'
      AND object_id = OBJECT_ID(N'dbo.Partners')
)
BEGIN
CREATE UNIQUE INDEX UX_Partners_CroatianPIN
    ON dbo.Partners (CroatianPIN)
    WHERE CroatianPIN IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Partners_CreatedAtUtc'
      AND object_id = OBJECT_ID(N'dbo.Partners')
)
BEGIN
CREATE INDEX IX_Partners_CreatedAtUtc
    ON dbo.Partners (CreatedAtUtc DESC);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Partners_IsDeleted'
      AND object_id = OBJECT_ID(N'dbo.Partners')
)
BEGIN
CREATE INDEX IX_Partners_IsDeleted
    ON dbo.Partners (IsDeleted);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Policies_PolicyNumber'
      AND object_id = OBJECT_ID(N'dbo.Policies')
)
BEGIN
CREATE UNIQUE INDEX UX_Policies_PolicyNumber
    ON dbo.Policies (PolicyNumber);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Policies_PartnerId'
      AND object_id = OBJECT_ID(N'dbo.Policies')
)
BEGIN
CREATE INDEX IX_Policies_PartnerId
    ON dbo.Policies (PartnerId);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Policies_IsDeleted'
      AND object_id = OBJECT_ID(N'dbo.Policies')
)
BEGIN
CREATE INDEX IX_Policies_IsDeleted
    ON dbo.Policies (IsDeleted);
END;