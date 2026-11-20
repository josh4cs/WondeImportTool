SET XACT_ABORT ON;
BEGIN TRANSACTION;

;WITH RankedClasses AS
(
	SELECT
		school_id, id, mis_id, name, code, description, subject, alternative,
		priority, academic_year, year_group, restored_at, created_at, updated_at, imported_at,
		ROW_NUMBER() OVER (PARTITION BY school_id, id ORDER BY imported_at DESC) AS row_number
	FROM dbo.classes_insert
), SourceClasses AS
(
	SELECT school_id, id, mis_id, name, code, description, subject, alternative,
		   priority, academic_year, year_group, restored_at, created_at, updated_at, imported_at
	FROM RankedClasses
	WHERE row_number = 1
)
MERGE dbo.classes AS target
USING SourceClasses AS source
	ON target.school_id = source.school_id
	AND target.id = source.id
WHEN MATCHED THEN
	UPDATE SET
		mis_id = source.mis_id,
		name = source.name,
		code = source.code,
		description = source.description,
		subject = source.subject,
		alternative = source.alternative,
		priority = source.priority,
		academic_year = source.academic_year,
		year_group = source.year_group,
		restored_at = source.restored_at,
		created_at = source.created_at,
		updated_at = source.updated_at,
		imported_at = source.imported_at
WHEN NOT MATCHED BY TARGET THEN
	INSERT (school_id, id, mis_id, name, code, description, subject, alternative, priority,
			academic_year, year_group, restored_at, created_at, updated_at, imported_at)
	VALUES (source.school_id, source.id, source.mis_id, source.name, source.code, source.description,
			source.subject, source.alternative, source.priority, source.academic_year, source.year_group,
			source.restored_at, source.created_at, source.updated_at, source.imported_at);

DELETE target
FROM dbo.classes_student AS target
INNER JOIN (SELECT DISTINCT school_id, class_id FROM dbo.classes_student_insert) AS source
	ON target.school_id = source.school_id
	AND target.class_id = source.class_id;

INSERT INTO dbo.classes_student (school_id, class_id, student_id)
SELECT DISTINCT school_id, class_id, student_id
FROM dbo.classes_student_insert;

DELETE target
FROM dbo.classes_employee AS target
INNER JOIN (SELECT DISTINCT school_id, class_id FROM dbo.classes_employee_insert) AS source
	ON target.school_id = source.school_id
	AND target.class_id = source.class_id;

INSERT INTO dbo.classes_employee
	(school_id, class_id, employee_id, role, is_main_teacher, is_class_teacher)
SELECT DISTINCT school_id, class_id, employee_id, role, is_main_teacher, is_class_teacher
FROM dbo.classes_employee_insert;

COMMIT TRANSACTION;
