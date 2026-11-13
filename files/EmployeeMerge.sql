SET XACT_ABORT ON;

BEGIN TRANSACTION;

;WITH RankedEmployees AS
(
	SELECT
		school_id,
		id,
		upi,
		mis_id,
		initials,
		title,
		surname,
		forename,
		middle_names,
		legal_surname,
		legal_forename,
		gender,
		date_of_birth,
		created_at,
		updated_at,
		imported_at,
		ROW_NUMBER() OVER
		(
			PARTITION BY school_id, id
			ORDER BY imported_at DESC
		) AS row_number
	FROM dbo.employees_insert
), SourceEmployees AS
(
	SELECT
		school_id,
		id,
		upi,
		mis_id,
		initials,
		title,
		surname,
		forename,
		middle_names,
		legal_surname,
		legal_forename,
		gender,
		date_of_birth,
		created_at,
		updated_at,
		imported_at
	FROM RankedEmployees
	WHERE row_number = 1
)
MERGE dbo.employees AS target
USING SourceEmployees AS source
	ON target.school_id = source.school_id
	AND target.id = source.id
WHEN MATCHED THEN
	UPDATE SET
		upi = source.upi,
		mis_id = source.mis_id,
		initials = source.initials,
		title = source.title,
		surname = source.surname,
		forename = source.forename,
		middle_names = source.middle_names,
		legal_surname = source.legal_surname,
		legal_forename = source.legal_forename,
		gender = source.gender,
		date_of_birth = source.date_of_birth,
		created_at = source.created_at,
		updated_at = source.updated_at,
		imported_at = source.imported_at
WHEN NOT MATCHED BY TARGET THEN
	INSERT
	(
		school_id,
		id,
		upi,
		mis_id,
		initials,
		title,
		surname,
		forename,
		middle_names,
		legal_surname,
		legal_forename,
		gender,
		date_of_birth,
		created_at,
		updated_at,
		imported_at,
		academic_year
	)
	VALUES
	(
		source.school_id,
		source.id,
		source.upi,
		source.mis_id,
		source.initials,
		source.title,
		source.surname,
		source.forename,
		source.middle_names,
		source.legal_surname,
		source.legal_forename,
		source.gender,
		source.date_of_birth,
		source.created_at,
		source.updated_at,
		source.imported_at,
		NULL
	);

DELETE target
FROM dbo.employment_details AS target
INNER JOIN
(
	SELECT DISTINCT school_id, employee_id
	FROM dbo.employment_details_insert
) AS source
	ON target.school_id = source.school_id
	AND target.employee_id = source.employee_id;

INSERT INTO dbo.employment_details
(
	school_id,
	employee_id,
	teaching_staff,
	[current],
	teacher_number,
	teacher_category,
	staff_code,
	primary_location,
	employment_start_date,
	employment_end_date,
	local_authority_start_date,
	employee_payroll_number,
	role_text,
	qualified_teacher_status,
	qualified_teacher_status_route,
	higher_level_teaching_assistant_status
)
SELECT DISTINCT
	school_id,
	employee_id,
	teaching_staff,
	[current],
	teacher_number,
	teacher_category,
	staff_code,
	primary_location,
	employment_start_date,
	employment_end_date,
	local_authority_start_date,
	employee_payroll_number,
	role_text,
	qualified_teacher_status,
	qualified_teacher_status_route,
	higher_level_teaching_assistant_status
FROM dbo.employment_details_insert;

DELETE target
FROM dbo.employee_email AS target
INNER JOIN
(
	SELECT DISTINCT school_id, employee_id
	FROM dbo.employee_email_insert
) AS source
	ON target.school_id = source.school_id
	AND target.employee_id = source.employee_id;

INSERT INTO dbo.employee_email
(
	school_id,
	employee_id,
	email,
	[primary],
	home,
	work
)
SELECT DISTINCT
	school_id,
	employee_id,
	email,
	[primary],
	home,
	work
FROM dbo.employee_email_insert;

COMMIT TRANSACTION;
