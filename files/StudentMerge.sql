SET XACT_ABORT ON;
BEGIN TRANSACTION;

DELETE target
FROM dbo.students AS target
INNER JOIN (SELECT DISTINCT school_id, id FROM dbo.students_insert) AS source
	ON target.school_id = source.school_id AND target.id = source.id;

;WITH Ranked AS
(
	SELECT *, ROW_NUMBER() OVER (PARTITION BY school_id, id ORDER BY imported_at DESC) AS rn
	FROM dbo.students_insert
)
INSERT INTO dbo.students
(
 school_id, academic_year, id, upi, mis_id, initials, title, surname, forename, middle_names, legal_surname, legal_forename,
 gender, gender_identity, date_of_birth, restored_at, created_at, updated_at, admission_number, upn, local_upn, former_upn,
 learner_number, current_nc_year, part_time, part_time_rate, admission_date, leaving_date, next_of_kin, religion, ethnicity,
 ethnicity_code, nationality, birth_place, custody_alert, custody_details, dietary_needs, paramedical_support, sen_status,
 first_language, marital_status, in_lea_care, ever_in_care, free_school_meals, free_school_meals_6,
 english_as_additional_language, english_as_additional_language_status, premium_pupil_indicator, premium_pupil_eligible,
 premium_pupil_notes, service_children_indicator, enrolment_status, enrolment_start_date, enrolment_end_date,
 gifted_and_talented_status, national_insurance_number, application_status, boarding_status, permanent_resident,
 in_care_details, general_notes, leaver_destination, fsm_review_date, youth_support_services_agreement,
 responsible_care_authority, home_language, child_protection_plan, child_in_need, imported_at
)
SELECT
 school_id, academic_year, id, upi, mis_id, initials, title, surname, forename, middle_names, legal_surname, legal_forename,
 gender, gender_identity, date_of_birth, restored_at, created_at, updated_at, admission_number, upn, local_upn, former_upn,
 learner_number, current_nc_year, part_time, part_time_rate, admission_date, leaving_date, next_of_kin, religion, ethnicity,
 ethnicity_code, nationality, birth_place, custody_alert, custody_details, dietary_needs, paramedical_support, sen_status,
 first_language, marital_status, in_lea_care, ever_in_care, free_school_meals, free_school_meals_6,
 english_as_additional_language, english_as_additional_language_status, premium_pupil_indicator, premium_pupil_eligible,
 premium_pupil_notes, service_children_indicator, enrolment_status, enrolment_start_date, enrolment_end_date,
 gifted_and_talented_status, national_insurance_number, application_status, boarding_status, permanent_resident,
 in_care_details, general_notes, leaver_destination, fsm_review_date, youth_support_services_agreement,
 responsible_care_authority, home_language, child_protection_plan, child_in_need, imported_at
FROM Ranked WHERE rn = 1;

DELETE target
FROM dbo.groups_students AS target
INNER JOIN (SELECT DISTINCT school_id, student_id FROM dbo.groups_students_insert) AS source
	ON target.school_id = source.school_id AND target.student_id = source.student_id;

INSERT INTO dbo.groups_students (school_id, student_id, group_id, start_date, end_date, imported_at)
SELECT DISTINCT school_id, student_id, group_id, start_date, end_date, imported_at
FROM dbo.groups_students_insert;

COMMIT TRANSACTION;
