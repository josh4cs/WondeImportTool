SET XACT_ABORT ON;

BEGIN TRANSACTION;

;WITH RankedGroups AS
(
	SELECT
		school_id,
		id,
		mis_id,
		name,
		code,
		type,
		description,
		notes,
		division,
		created_at,
		updated_at,
		imported_at,
		ROW_NUMBER() OVER
		(
			PARTITION BY school_id, id
			ORDER BY imported_at DESC
		) AS row_number
	FROM dbo.groups_insert
), SourceGroups AS
(
	SELECT
		school_id,
		id,
		mis_id,
		name,
		code,
		type,
		description,
		notes,
		division,
		created_at,
		updated_at,
		imported_at
	FROM RankedGroups
	WHERE row_number = 1
)
MERGE dbo.groups AS target
USING SourceGroups AS source
	ON target.school_id = source.school_id
	AND target.id = source.id
WHEN MATCHED THEN
	UPDATE SET
		mis_id = source.mis_id,
		name = source.name,
		code = source.code,
		type = source.type,
		description = source.description,
		notes = source.notes,
		division = source.division,
		created_at = source.created_at,
		updated_at = source.updated_at,
		imported_at = source.imported_at
WHEN NOT MATCHED BY TARGET THEN
	INSERT
	(
		school_id,
		id,
		mis_id,
		name,
		code,
		type,
		description,
		notes,
		division,
		created_at,
		updated_at,
		imported_at
	)
	VALUES
	(
		source.school_id,
		source.id,
		source.mis_id,
		source.name,
		source.code,
		source.type,
		source.description,
		source.notes,
		source.division,
		source.created_at,
		source.updated_at,
		source.imported_at
	);

DELETE target
FROM dbo.groups_students AS target
INNER JOIN
(
	SELECT DISTINCT school_id, group_id
	FROM dbo.groups_students_insert
) AS source
	ON target.school_id = source.school_id
	AND target.group_id = source.group_id;

INSERT INTO dbo.groups_students
(
	school_id,
	student_id,
	group_id,
	start_date,
	end_date,
	imported_at
)
SELECT DISTINCT
	school_id,
	student_id,
	group_id,
	start_date,
	end_date,
	imported_at
FROM dbo.groups_students_insert;

DELETE target
FROM dbo.groups_employees AS target
INNER JOIN
(
	SELECT DISTINCT school_id, group_id
	FROM dbo.groups_employees_insert
) AS source
	ON target.school_id = source.school_id
	AND target.group_id = source.group_id;

INSERT INTO dbo.groups_employees
(
	school_id,
	employee_id,
	group_id,
	role
)
SELECT DISTINCT
	school_id,
	employee_id,
	group_id,
	role
FROM dbo.groups_employees_insert;

COMMIT TRANSACTION;
