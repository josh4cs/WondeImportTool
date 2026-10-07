SET XACT_ABORT ON;

BEGIN TRANSACTION;

;WITH RankedBehaviours AS
(
	SELECT
		school_id,
		id,
		behaviour_type,
		location,
		subject,
		[class],
		status,
		action,
		comment,
		parents_notified,
		points,
		incident_date,
		action_date,
		created_on,
		recorded_on,
		updated_on,
		imported_on,
		ROW_NUMBER() OVER
		(
			PARTITION BY school_id, id
			ORDER BY imported_on DESC
		) AS row_number
	FROM dbo.behaviours_table_insert
), SourceBehaviours AS
(
	SELECT
		school_id,
		id,
		behaviour_type,
		location,
		subject,
		[class],
		status,
		action,
		comment,
		parents_notified,
		points,
		incident_date,
		action_date,
		created_on,
		recorded_on,
		updated_on,
		imported_on
	FROM RankedBehaviours
	WHERE row_number = 1
)
MERGE dbo.behaviours_table AS target
USING SourceBehaviours AS source
	ON target.school_id = source.school_id
	AND target.id = source.id
WHEN MATCHED THEN
	UPDATE SET
		behaviour_type = source.behaviour_type,
		location = source.location,
		subject = source.subject,
		[class] = source.[class],
		status = source.status,
		action = source.action,
		comment = source.comment,
		parents_notified = source.parents_notified,
		points = source.points,
		incident_date = source.incident_date,
		action_date = source.action_date,
		created_on = source.created_on,
		recorded_on = source.recorded_on,
		updated_on = source.updated_on,
		imported_on = source.imported_on
WHEN NOT MATCHED BY TARGET THEN
	INSERT
	(
		school_id,
		id,
		behaviour_type,
		location,
		subject,
		[class],
		status,
		action,
		comment,
		parents_notified,
		points,
		incident_date,
		action_date,
		created_on,
		recorded_on,
		updated_on,
		imported_on
	)
	VALUES
	(
		source.school_id,
		source.id,
		source.behaviour_type,
		source.location,
		source.subject,
		source.[class],
		source.status,
		source.action,
		source.comment,
		source.parents_notified,
		source.points,
		source.incident_date,
		source.action_date,
		source.created_on,
		source.recorded_on,
		source.updated_on,
		source.imported_on
	);

DELETE target
FROM dbo.behaviours_students AS target
INNER JOIN
(
	SELECT DISTINCT school_id, id
	FROM dbo.behaviours_table_insert
) AS source
	ON target.school_id = source.school_id
	AND target.id = source.id;

INSERT INTO dbo.behaviours_students
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
FROM dbo.behaviours_students_insert;

COMMIT TRANSACTION;
