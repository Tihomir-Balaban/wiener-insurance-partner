MERGE dbo.PartnerTypes AS target
    USING
    (
    VALUES
    (1, N'Personal'),
    (2, N'Legal')
    ) AS source (Id, Name)
    ON target.Id = source.Id
    WHEN MATCHED AND target.Name <> source.Name THEN
UPDATE SET Name = source.Name
    WHEN NOT MATCHED BY TARGET THEN
INSERT (Id, Name)
VALUES (source.Id, source.Name);