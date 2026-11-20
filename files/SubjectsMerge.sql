SET XACT_ABORT ON;
BEGIN TRANSACTION;

;WITH RankedSubjects AS
(
	SELECT
		school_id, id, mis_id, code, name, active, created_at, updated_at, imported_at,
		ROW_NUMBER() OVER (PARTITION BY school_id, id ORDER BY imported_at DESC) AS row_number
	FROM dbo.subjects_insert
), SourceSubjects AS
(
	SELECT school_id, id, mis_id, code, name, active, created_at, updated_at, imported_at
	FROM RankedSubjects
	WHERE row_number = 1
)
MERGE dbo.subjects AS target
USING SourceSubjects AS source
	ON target.school_id = source.school_id
	AND target.id = source.id
WHEN MATCHED THEN
	UPDATE SET
		mis_id = source.mis_id,
		code = source.code,
		name = source.name,
		active = source.active,
		created_at = source.created_at,
		updated_at = source.updated_at,
		imported_at = source.imported_at
WHEN NOT MATCHED BY TARGET THEN
	INSERT (school_id, id, mis_id, code, name, active, created_at, updated_at, imported_at)
	VALUES (source.school_id, source.id, source.mis_id, source.code, source.name, source.active,
			source.created_at, source.updated_at, source.imported_at);

COMMIT TRANSACTION;
