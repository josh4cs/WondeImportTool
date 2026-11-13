SET XACT_ABORT ON;

BEGIN TRANSACTION;

;WITH RankedAttendance AS
(
	SELECT
		school_id,
		id,
		[date],
		comment,
		session,
		employee_id,
		attendance_code_id,
		student_id,
		imported_at,
		ROW_NUMBER() OVER
		(
			PARTITION BY school_id, id
			ORDER BY imported_at DESC
		) AS row_number
	FROM dbo.attendance_session_insert
), SourceAttendance AS
(
	SELECT
		school_id,
		id,
		[date],
		comment,
		session,
		employee_id,
		attendance_code_id,
		student_id,
		imported_at
	FROM RankedAttendance
	WHERE row_number = 1
)
MERGE dbo.attendance_session AS target
USING SourceAttendance AS source
	ON target.school_id = source.school_id
	AND target.id = source.id
WHEN MATCHED THEN
	UPDATE SET
		[date] = source.[date],
		comment = source.comment,
		session = source.session,
		employee_id = source.employee_id,
		attendance_code_id = source.attendance_code_id,
		student_id = source.student_id,
		imported_at = source.imported_at
WHEN NOT MATCHED BY TARGET THEN
	INSERT
	(
		school_id,
		id,
		[date],
		comment,
		session,
		employee_id,
		attendance_code_id,
		student_id,
		imported_at
	)
	VALUES
	(
		source.school_id,
		source.id,
		source.[date],
		source.comment,
		source.session,
		source.employee_id,
		source.attendance_code_id,
		source.student_id,
		source.imported_at
	);

COMMIT TRANSACTION;
