SET XACT_ABORT ON;

BEGIN TRANSACTION;

;WITH RankedAchievements AS
(
	SELECT
		school_id,
		id,
		achievement_type,
		action,
		subject,
		[class],
		points,
		comments,
		parents_notified,
		achievement_date,
		action_date,
		recorded_date,
		created_at,
		updated_at,
		imported_at,
		ROW_NUMBER() OVER
		(
			PARTITION BY school_id, id
			ORDER BY imported_at DESC
		) AS row_number
	FROM dbo.achievements_table_insert
), SourceAchievements AS
(
	SELECT
		school_id,
		id,
		achievement_type,
		action,
		subject,
		[class],
		points,
		comments,
		parents_notified,
		achievement_date,
		action_date,
		recorded_date,
		created_at,
		updated_at,
		imported_at
	FROM RankedAchievements
	WHERE row_number = 1
)
MERGE dbo.achievements_table AS target
USING SourceAchievements AS source
	ON target.school_id = source.school_id
	AND target.id = source.id
WHEN MATCHED THEN
	UPDATE SET
		achievement_type = source.achievement_type,
		action = source.action,
		subject = source.subject,
		[class] = source.[class],
		points = source.points,
		comments = source.comments,
		parents_notified = source.parents_notified,
		achievement_date = source.achievement_date,
		action_date = source.action_date,
		recorded_date = source.recorded_date,
		created_at = source.created_at,
		updated_at = source.updated_at,
		imported_at = source.imported_at
WHEN NOT MATCHED BY TARGET THEN
	INSERT
	(
		school_id,
		id,
		achievement_type,
		action,
		subject,
		[class],
		points,
		comments,
		parents_notified,
		achievement_date,
		action_date,
		recorded_date,
		created_at,
		updated_at,
		imported_at
	)
	VALUES
	(
		source.school_id,
		source.id,
		source.achievement_type,
		source.action,
		source.subject,
		source.[class],
		source.points,
		source.comments,
		source.parents_notified,
		source.achievement_date,
		source.action_date,
		source.recorded_date,
		source.created_at,
		source.updated_at,
		source.imported_at
	);

DELETE target
FROM dbo.achievements_students AS target
INNER JOIN
(
	SELECT DISTINCT school_id, id
	FROM dbo.achievements_table_insert
) AS source
	ON target.school_id = source.school_id
	AND target.id = source.id;

INSERT INTO dbo.achievements_students
(
	school_id,
	id,
	student_id,
	points,
	points_meta
)
SELECT DISTINCT
	school_id,
	id,
	student_id,
	points,
	points_meta
FROM dbo.achievements_students_insert;

COMMIT TRANSACTION;
