SET XACT_ABORT ON;

BEGIN TRANSACTION;

;WITH RankedExclusions AS
(
	SELECT
		school_id,
		id,
		type,
		type_code,
		reason,
		reason_code,
		start_date,
		start_session,
		end_date,
		end_session,
		sessions,
		days,
		academic_year,
		term,
		comments,
		agencies_involved,
		discipline_committee_date,
		discipline_committee_result,
		discipline_committee_reinstatement_date,
		discipline_committee_representation_made,
		appeal_received,
		appeal_result,
		appeal_result_date,
		appeal_reinstatement_date,
		created_at,
		updated_at,
		imported_at,
		ROW_NUMBER() OVER
		(
			PARTITION BY school_id, id
			ORDER BY imported_at DESC
		) AS row_number
	FROM dbo.exclusion_table_insert
), SourceExclusions AS
(
	SELECT
		school_id,
		id,
		type,
		type_code,
		reason,
		reason_code,
		start_date,
		start_session,
		end_date,
		end_session,
		sessions,
		days,
		academic_year,
		term,
		comments,
		agencies_involved,
		discipline_committee_date,
		discipline_committee_result,
		discipline_committee_reinstatement_date,
		discipline_committee_representation_made,
		appeal_received,
		appeal_result,
		appeal_result_date,
		appeal_reinstatement_date,
		created_at,
		updated_at,
		imported_at
	FROM RankedExclusions
	WHERE row_number = 1
)
MERGE dbo.exclusion_table AS target
USING SourceExclusions AS source
	ON target.school_id = source.school_id
	AND target.id = source.id
WHEN MATCHED THEN
	UPDATE SET
		type = source.type,
		type_code = source.type_code,
		reason = source.reason,
		reason_code = source.reason_code,
		start_date = source.start_date,
		start_session = source.start_session,
		end_date = source.end_date,
		end_session = source.end_session,
		sessions = source.sessions,
		days = source.days,
		academic_year = source.academic_year,
		term = source.term,
		comments = source.comments,
		agencies_involved = source.agencies_involved,
		discipline_committee_date = source.discipline_committee_date,
		discipline_committee_result = source.discipline_committee_result,
		discipline_committee_reinstatement_date = source.discipline_committee_reinstatement_date,
		discipline_committee_representation_made = source.discipline_committee_representation_made,
		appeal_received = source.appeal_received,
		appeal_result = source.appeal_result,
		appeal_result_date = source.appeal_result_date,
		appeal_reinstatement_date = source.appeal_reinstatement_date,
		created_at = source.created_at,
		updated_at = source.updated_at,
		imported_at = source.imported_at
WHEN NOT MATCHED BY TARGET THEN
	INSERT
	(
		school_id,
		id,
		type,
		type_code,
		reason,
		reason_code,
		start_date,
		start_session,
		end_date,
		end_session,
		sessions,
		days,
		academic_year,
		term,
		comments,
		agencies_involved,
		discipline_committee_date,
		discipline_committee_result,
		discipline_committee_reinstatement_date,
		discipline_committee_representation_made,
		appeal_received,
		appeal_result,
		appeal_result_date,
		appeal_reinstatement_date,
		created_at,
		updated_at,
		imported_at
	)
	VALUES
	(
		source.school_id,
		source.id,
		source.type,
		source.type_code,
		source.reason,
		source.reason_code,
		source.start_date,
		source.start_session,
		source.end_date,
		source.end_session,
		source.sessions,
		source.days,
		source.academic_year,
		source.term,
		source.comments,
		source.agencies_involved,
		source.discipline_committee_date,
		source.discipline_committee_result,
		source.discipline_committee_reinstatement_date,
		source.discipline_committee_representation_made,
		source.appeal_received,
		source.appeal_result,
		source.appeal_result_date,
		source.appeal_reinstatement_date,
		source.created_at,
		source.updated_at,
		source.imported_at
	);

DELETE target
FROM dbo.exclusion_students AS target
INNER JOIN
(
	SELECT DISTINCT school_id, id
	FROM dbo.exclusion_table_insert
) AS source
	ON target.school_id = source.school_id
	AND target.id = source.id;

INSERT INTO dbo.exclusion_students
(
	school_id,
	id,
	student_id
)
SELECT DISTINCT
	school_id,
	id,
	student_id
FROM dbo.exclusion_students_insert;

COMMIT TRANSACTION;
