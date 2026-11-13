USE NewDatabase
GO

CREATE TABLE [dbo].[achievements_students](
	[school_id] [varchar](50) NOT NULL,
	[id] [varchar](50) NULL,
	[student_id] [varchar](50) NULL,
	[points] [int] NULL,
	[points_meta] [int] NULL
) 
GO


CREATE TABLE [dbo].[achievements_students_insert](
	[school_id] [varchar](50) NOT NULL,
	[id] [varchar](50) NULL,
	[student_id] [varchar](50) NULL,
	[points] [int] NULL,
	[points_meta] [int] NULL
) 
GO


CREATE TABLE [dbo].[achievements_table](
	[school_id] [varchar](50) NOT NULL,
	[id] [varchar](50) NULL,
	[achievement_type] [varchar](100) NULL,
	[action] [varchar](100) NULL,
	[subject] [varchar](100) NULL,
	[class] [varchar](100) NULL,
	[points] [int] NULL,
	[comments] [varchar](max) NULL,
	[parents_notified] [bit] NULL,
	[achievement_date] [datetime] NULL,
	[action_date] [datetime] NULL,
	[recorded_date] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO



CREATE TABLE [dbo].[achievements_table_insert](
	[school_id] [varchar](50) NOT NULL,
	[id] [varchar](50) NULL,
	[achievement_type] [varchar](100) NULL,
	[action] [varchar](100) NULL,
	[subject] [varchar](100) NULL,
	[class] [varchar](100) NULL,
	[points] [int] NULL,
	[comments] [varchar](max) NULL,
	[parents_notified] [bit] NULL,
	[achievement_date] [datetime] NULL,
	[action_date] [datetime] NULL,
	[recorded_date] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[attendance_session](
	[school_id] [nvarchar](50) NULL,
	[id] [nvarchar](50) NULL,
	[date] [date] NULL,
	[comment] [nvarchar](max) NULL,
	[session] [nvarchar](2) NULL,
	[employee_id] [nvarchar](50) NULL,
	[attendance_code_id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[imported_at] [smalldatetime] NULL
) 
GO


CREATE TABLE [dbo].[attendance_session_insert](
	[school_id] [nvarchar](50) NULL,
	[id] [nvarchar](50) NULL,
	[date] [date] NULL,
	[comment] [nvarchar](max) NULL,
	[session] [nvarchar](2) NULL,
	[employee_id] [nvarchar](50) NULL,
	[attendance_code_id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[imported_at] [smalldatetime] NULL
) 
GO


CREATE TABLE [dbo].[behaviours_students](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[points] [int] NULL,
	[points_meta] [int] NULL
) 
GO


CREATE TABLE [dbo].[behaviours_students_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[points] [int] NULL,
	[points_meta] [int] NULL
) 
GO


CREATE TABLE [dbo].[behaviours_table](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[behaviour_type] [varchar](100) NULL,
	[location] [varchar](100) NULL,
	[subject] [varchar](100) NULL,
	[class] [varchar](100) NULL,
	[status] [varchar](100) NULL,
	[action] [varchar](100) NULL,
	[comment] [varchar](max) NULL,
	[parents_notified] [varchar](100) NULL,
	[points] [int] NULL,
	[incident_date] [datetime] NULL,
	[action_date] [datetime] NULL,
	[created_on] [datetime] NULL,
	[recorded_on] [datetime] NULL,
	[updated_on] [datetime] NULL,
	[imported_on] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[behaviours_table_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[behaviour_type] [varchar](100) NULL,
	[location] [varchar](100) NULL,
	[subject] [varchar](100) NULL,
	[class] [varchar](100) NULL,
	[status] [varchar](100) NULL,
	[action] [varchar](100) NULL,
	[comment] [varchar](max) NULL,
	[parents_notified] [varchar](100) NULL,
	[points] [int] NULL,
	[incident_date] [datetime] NULL,
	[action_date] [datetime] NULL,
	[created_on] [datetime] NULL,
	[recorded_on] [datetime] NULL,
	[updated_on] [datetime] NULL,
	[imported_on] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[employee_email](
	[school_id] [nvarchar](50) NULL,
	[employee_id] [nvarchar](50) NULL,
	[email] [nvarchar](255) NULL,
	[primary] [nvarchar](255) NULL,
	[home] [nvarchar](255) NULL,
	[work] [nvarchar](255) NULL
) 
GO


CREATE TABLE [dbo].[employee_email_insert](
	[school_id] [nvarchar](50) NULL,
	[employee_id] [nvarchar](50) NULL,
	[email] [nvarchar](255) NULL,
	[primary] [nvarchar](255) NULL,
	[home] [nvarchar](255) NULL,
	[work] [nvarchar](255) NULL
) 
GO


CREATE TABLE [dbo].[employees](
	[school_id] [nvarchar](50) NULL,
	[id] [nvarchar](50) NULL,
	[upi] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[initials] [nvarchar](max) NULL,
	[title] [nvarchar](max) NULL,
	[surname] [nvarchar](max) NULL,
	[forename] [nvarchar](max) NULL,
	[middle_names] [nvarchar](max) NULL,
	[legal_surname] [nvarchar](max) NULL,
	[legal_forename] [nvarchar](max) NULL,
	[gender] [nvarchar](50) NULL,
	[date_of_birth] [date] NULL,
	[created_at] [smalldatetime] NULL,
	[updated_at] [smalldatetime] NULL,
	[imported_at] [smalldatetime] NULL,
	[academic_year] [nvarchar](50) NULL
) 
GO


CREATE TABLE [dbo].[employees_insert](
	[school_id] [nvarchar](50) NULL,
	[id] [nvarchar](50) NULL,
	[upi] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[initials] [nvarchar](max) NULL,
	[title] [nvarchar](max) NULL,
	[surname] [nvarchar](max) NULL,
	[forename] [nvarchar](max) NULL,
	[middle_names] [nvarchar](max) NULL,
	[legal_surname] [nvarchar](max) NULL,
	[legal_forename] [nvarchar](max) NULL,
	[gender] [nvarchar](50) NULL,
	[date_of_birth] [date] NULL,
	[created_at] [smalldatetime] NULL,
	[updated_at] [smalldatetime] NULL,
	[imported_at] [smalldatetime] NULL
) 
GO


CREATE TABLE [dbo].[employment_details](
	[school_id] [nvarchar](50) NULL,
	[employee_id] [nvarchar](50) NULL,
	[teaching_staff] [bit] NULL,
	[current] [bit] NULL,
	[teacher_number] [nvarchar](255) NULL,
	[teacher_category] [nvarchar](255) NULL,
	[staff_code] [nvarchar](255) NULL,
	[primary_location] [nvarchar](255) NULL,
	[employment_start_date] [date] NULL,
	[employment_end_date] [date] NULL,
	[local_authority_start_date] [date] NULL,
	[employee_payroll_number] [nvarchar](255) NULL,
	[role_text] [nvarchar](255) NULL,
	[qualified_teacher_status] [nvarchar](255) NULL,
	[qualified_teacher_status_route] [nvarchar](255) NULL,
	[higher_level_teaching_assistant_status] [nvarchar](255) NULL
) 
GO


CREATE TABLE [dbo].[employment_details_insert](
	[school_id] [nvarchar](50) NULL,
	[employee_id] [nvarchar](50) NULL,
	[teaching_staff] [bit] NULL,
	[current] [bit] NULL,
	[teacher_number] [nvarchar](255) NULL,
	[teacher_category] [nvarchar](255) NULL,
	[staff_code] [nvarchar](255) NULL,
	[primary_location] [nvarchar](255) NULL,
	[employment_start_date] [date] NULL,
	[employment_end_date] [date] NULL,
	[local_authority_start_date] [date] NULL,
	[employee_payroll_number] [nvarchar](255) NULL,
	[role_text] [nvarchar](255) NULL,
	[qualified_teacher_status] [nvarchar](255) NULL,
	[qualified_teacher_status_route] [nvarchar](255) NULL,
	[higher_level_teaching_assistant_status] [nvarchar](255) NULL
) 
GO


CREATE TABLE [dbo].[exclusion_students](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL
) 
GO


CREATE TABLE [dbo].[exclusion_students_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL
) 
GO


CREATE TABLE [dbo].[exclusion_table](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[type] [nvarchar](50) NULL,
	[type_code] [nvarchar](50) NULL,
	[reason] [nvarchar](255) NULL,
	[reason_code] [nvarchar](255) NULL,
	[start_date] [datetime] NULL,
	[start_session] [nvarchar](50) NULL,
	[end_date] [datetime] NULL,
	[end_session] [nvarchar](50) NULL,
	[sessions] [nvarchar](50) NULL,
	[days] [nvarchar](50) NULL,
	[academic_year] [nvarchar](50) NULL,
	[term] [nvarchar](50) NULL,
	[comments] [nvarchar](max) NULL,
	[agencies_involved] [nvarchar](255) NULL,
	[discipline_committee_date] [datetime] NULL,
	[discipline_committee_result] [nvarchar](255) NULL,
	[discipline_committee_reinstatement_date] [datetime] NULL,
	[discipline_committee_representation_made] [bit] NULL,
	[appeal_received] [bit] NULL,
	[appeal_result] [nvarchar](255) NULL,
	[appeal_result_date] [datetime] NULL,
	[appeal_reinstatement_date] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[exclusion_table_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[type] [nvarchar](50) NULL,
	[type_code] [nvarchar](50) NULL,
	[reason] [nvarchar](255) NULL,
	[reason_code] [nvarchar](255) NULL,
	[start_date] [datetime] NULL,
	[start_session] [nvarchar](50) NULL,
	[end_date] [datetime] NULL,
	[end_session] [nvarchar](50) NULL,
	[sessions] [nvarchar](50) NULL,
	[days] [nvarchar](50) NULL,
	[academic_year] [nvarchar](50) NULL,
	[term] [nvarchar](50) NULL,
	[comments] [nvarchar](max) NULL,
	[agencies_involved] [nvarchar](255) NULL,
	[discipline_committee_date] [datetime] NULL,
	[discipline_committee_result] [nvarchar](255) NULL,
	[discipline_committee_reinstatement_date] [datetime] NULL,
	[discipline_committee_representation_made] [bit] NULL,
	[appeal_received] [bit] NULL,
	[appeal_result] [nvarchar](255) NULL,
	[appeal_result_date] [datetime] NULL,
	[appeal_reinstatement_date] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[groups](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[name] [nvarchar](100) NULL,
	[code] [nvarchar](100) NULL,
	[type] [nvarchar](100) NULL,
	[description] [nvarchar](max) NULL,
	[notes] [nvarchar](max) NULL,
	[division] [nvarchar](100) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[groups_employees](
	[school_id] [nvarchar](50) NOT NULL,
	[employee_id] [nvarchar](50) NULL,
	[group_id] [nvarchar](50) NULL,
	[role] [nvarchar](50) NULL
) 
GO


CREATE TABLE [dbo].[groups_employees_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[employee_id] [nvarchar](50) NULL,
	[group_id] [nvarchar](50) NULL,
	[role] [nvarchar](50) NULL
) 
GO


CREATE TABLE [dbo].[groups_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[name] [nvarchar](100) NULL,
	[code] [nvarchar](100) NULL,
	[type] [nvarchar](100) NULL,
	[description] [nvarchar](max) NULL,
	[notes] [nvarchar](max) NULL,
	[division] [nvarchar](100) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[groups_students](
	[school_id] [nvarchar](50) NOT NULL,
	[student_id] [nvarchar](50) NULL,
	[group_id] [nvarchar](50) NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[groups_students_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[student_id] [nvarchar](50) NULL,
	[group_id] [nvarchar](50) NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[IMPORT_LOG](
	[school_id] [varchar](50) NOT NULL,
	[request_id] [varchar](50) NULL,
	[table_name] [varchar](50) NULL,
	[instance] [varchar](50) NULL,
	[url] [nvarchar](max) NULL,
	[start_time] [datetime] NULL,
	[end_time] [datetime] NULL,
	[rows_processed] [int] NULL
) 
GO



CREATE TABLE [dbo].[student_leaver](
	[school_id] [nvarchar](50) NULL,
	[academic_year] [nvarchar](50) NULL,
	[id] [nvarchar](50) NULL,
	[upi] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[initials] [nvarchar](50) NULL,
	[title] [nvarchar](max) NULL,
	[surname] [nvarchar](max) NULL,
	[forename] [nvarchar](max) NULL,
	[middle_names] [nvarchar](max) NULL,
	[legal_surname] [nvarchar](max) NULL,
	[legal_forename] [nvarchar](max) NULL,
	[gender] [nvarchar](50) NULL,
	[gender_identity] [nvarchar](50) NULL,
	[date_of_birth] [date] NULL,
	[restored_at] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[admission_number] [nvarchar](50) NULL,
	[upn] [nvarchar](50) NULL,
	[local_upn] [nvarchar](50) NULL,
	[former_upn] [nvarchar](50) NULL,
	[learner_number] [nvarchar](50) NULL,
	[current_nc_year] [nvarchar](50) NULL,
	[part_time] [nvarchar](50) NULL,
	[part_time_rate] [nvarchar](50) NULL,
	[admission_date] [date] NULL,
	[leaving_date] [date] NULL,
	[next_of_kin] [nvarchar](50) NULL,
	[religion] [nvarchar](50) NULL,
	[ethnicity] [nvarchar](50) NULL,
	[ethnicity_code] [nvarchar](50) NULL,
	[nationality] [nvarchar](50) NULL,
	[birth_place] [nvarchar](50) NULL,
	[custody_alert] [nvarchar](50) NULL,
	[custody_details] [nvarchar](50) NULL,
	[dietary_needs] [nvarchar](255) NULL,
	[paramedical_support] [nvarchar](50) NULL,
	[sen_status] [nvarchar](50) NULL,
	[first_language] [nvarchar](50) NULL,
	[marital_status] [nvarchar](50) NULL,
	[in_lea_care] [bit] NULL,
	[ever_in_care] [bit] NULL,
	[free_school_meals] [bit] NULL,
	[free_school_meals_6] [bit] NULL,
	[english_as_additional_language] [bit] NULL,
	[english_as_additional_language_status] [nvarchar](50) NULL,
	[premium_pupil_indicator] [bit] NULL,
	[premium_pupil_eligible] [bit] NULL,
	[premium_pupil_notes] [nvarchar](255) NULL,
	[service_children_indicator] [bit] NULL,
	[enrolment_status] [nvarchar](50) NULL,
	[enrolment_start_date] [date] NULL,
	[enrolment_end_date] [date] NULL,
	[gifted_and_talented_status] [bit] NULL,
	[national_insurance_number] [nvarchar](50) NULL,
	[application_status] [nvarchar](50) NULL,
	[boarding_status] [nvarchar](50) NULL,
	[permanent_resident] [nvarchar](50) NULL,
	[in_care_details] [nvarchar](50) NULL,
	[general_notes] [nvarchar](255) NULL,
	[leaver_destination] [nvarchar](255) NULL,
	[fsm_review_date] [nvarchar](50) NULL,
	[youth_support_services_agreement] [nvarchar](50) NULL,
	[responsible_care_authority] [nvarchar](50) NULL,
	[home_language] [nvarchar](50) NULL,
	[child_protection_plan] [bit] NULL,
	[child_in_need] [bit] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[student_leaver_cdbs](
	[school_id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[N00595] [nvarchar](50) NULL
) 
GO


CREATE TABLE [dbo].[student_leaver_cdbs_insert](
	[school_id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[N00595] [nvarchar](50) NULL
) 
GO


CREATE TABLE [dbo].[student_leaver_insert](
	[school_id] [nvarchar](50) NULL,
	[academic_year] [nvarchar](50) NULL,
	[id] [nvarchar](50) NULL,
	[upi] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[initials] [nvarchar](50) NULL,
	[title] [nvarchar](max) NULL,
	[surname] [nvarchar](max) NULL,
	[forename] [nvarchar](max) NULL,
	[middle_names] [nvarchar](max) NULL,
	[legal_surname] [nvarchar](max) NULL,
	[legal_forename] [nvarchar](max) NULL,
	[gender] [nvarchar](50) NULL,
	[gender_identity] [nvarchar](50) NULL,
	[date_of_birth] [date] NULL,
	[restored_at] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[admission_number] [nvarchar](50) NULL,
	[upn] [nvarchar](50) NULL,
	[local_upn] [nvarchar](50) NULL,
	[former_upn] [nvarchar](50) NULL,
	[learner_number] [nvarchar](50) NULL,
	[current_nc_year] [nvarchar](50) NULL,
	[part_time] [nvarchar](50) NULL,
	[part_time_rate] [nvarchar](50) NULL,
	[admission_date] [date] NULL,
	[leaving_date] [date] NULL,
	[next_of_kin] [nvarchar](50) NULL,
	[religion] [nvarchar](50) NULL,
	[ethnicity] [nvarchar](50) NULL,
	[ethnicity_code] [nvarchar](50) NULL,
	[nationality] [nvarchar](50) NULL,
	[birth_place] [nvarchar](50) NULL,
	[custody_alert] [nvarchar](50) NULL,
	[custody_details] [nvarchar](50) NULL,
	[dietary_needs] [nvarchar](255) NULL,
	[paramedical_support] [nvarchar](50) NULL,
	[sen_status] [nvarchar](50) NULL,
	[first_language] [nvarchar](50) NULL,
	[marital_status] [nvarchar](50) NULL,
	[in_lea_care] [bit] NULL,
	[ever_in_care] [bit] NULL,
	[free_school_meals] [bit] NULL,
	[free_school_meals_6] [bit] NULL,
	[english_as_additional_language] [bit] NULL,
	[english_as_additional_language_status] [nvarchar](50) NULL,
	[premium_pupil_indicator] [bit] NULL,
	[premium_pupil_eligible] [bit] NULL,
	[premium_pupil_notes] [nvarchar](255) NULL,
	[service_children_indicator] [bit] NULL,
	[enrolment_status] [nvarchar](50) NULL,
	[enrolment_start_date] [date] NULL,
	[enrolment_end_date] [date] NULL,
	[gifted_and_talented_status] [bit] NULL,
	[national_insurance_number] [nvarchar](50) NULL,
	[application_status] [nvarchar](50) NULL,
	[boarding_status] [nvarchar](50) NULL,
	[permanent_resident] [nvarchar](50) NULL,
	[in_care_details] [nvarchar](50) NULL,
	[general_notes] [nvarchar](255) NULL,
	[leaver_destination] [nvarchar](255) NULL,
	[fsm_review_date] [nvarchar](50) NULL,
	[youth_support_services_agreement] [nvarchar](50) NULL,
	[responsible_care_authority] [nvarchar](50) NULL,
	[home_language] [nvarchar](50) NULL,
	[child_protection_plan] [bit] NULL,
	[child_in_need] [bit] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[students](
	[school_id] [nvarchar](50) NULL,
	[academic_year] [nvarchar](50) NULL,
	[id] [nvarchar](50) NULL,
	[upi] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[initials] [nvarchar](50) NULL,
	[title] [nvarchar](max) NULL,
	[surname] [nvarchar](max) NULL,
	[forename] [nvarchar](max) NULL,
	[middle_names] [nvarchar](max) NULL,
	[legal_surname] [nvarchar](max) NULL,
	[legal_forename] [nvarchar](max) NULL,
	[gender] [nvarchar](50) NULL,
	[gender_identity] [nvarchar](50) NULL,
	[date_of_birth] [date] NULL,
	[restored_at] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[admission_number] [nvarchar](50) NULL,
	[upn] [nvarchar](50) NULL,
	[local_upn] [nvarchar](50) NULL,
	[former_upn] [nvarchar](50) NULL,
	[learner_number] [nvarchar](50) NULL,
	[current_nc_year] [nvarchar](50) NULL,
	[part_time] [nvarchar](50) NULL,
	[part_time_rate] [nvarchar](50) NULL,
	[admission_date] [date] NULL,
	[leaving_date] [date] NULL,
	[next_of_kin] [nvarchar](50) NULL,
	[religion] [nvarchar](50) NULL,
	[ethnicity] [nvarchar](50) NULL,
	[ethnicity_code] [nvarchar](50) NULL,
	[nationality] [nvarchar](50) NULL,
	[birth_place] [nvarchar](50) NULL,
	[custody_alert] [nvarchar](50) NULL,
	[custody_details] [nvarchar](50) NULL,
	[dietary_needs] [nvarchar](255) NULL,
	[paramedical_support] [nvarchar](50) NULL,
	[sen_status] [nvarchar](50) NULL,
	[first_language] [nvarchar](50) NULL,
	[marital_status] [nvarchar](50) NULL,
	[in_lea_care] [bit] NULL,
	[ever_in_care] [bit] NULL,
	[free_school_meals] [bit] NULL,
	[free_school_meals_6] [bit] NULL,
	[english_as_additional_language] [bit] NULL,
	[english_as_additional_language_status] [nvarchar](50) NULL,
	[premium_pupil_indicator] [bit] NULL,
	[premium_pupil_eligible] [bit] NULL,
	[premium_pupil_notes] [nvarchar](255) NULL,
	[service_children_indicator] [bit] NULL,
	[enrolment_status] [nvarchar](50) NULL,
	[enrolment_start_date] [date] NULL,
	[enrolment_end_date] [date] NULL,
	[gifted_and_talented_status] [bit] NULL,
	[national_insurance_number] [nvarchar](50) NULL,
	[application_status] [nvarchar](50) NULL,
	[boarding_status] [nvarchar](50) NULL,
	[permanent_resident] [nvarchar](50) NULL,
	[in_care_details] [nvarchar](50) NULL,
	[general_notes] [nvarchar](255) NULL,
	[leaver_destination] [nvarchar](255) NULL,
	[fsm_review_date] [nvarchar](50) NULL,
	[youth_support_services_agreement] [nvarchar](50) NULL,
	[responsible_care_authority] [nvarchar](50) NULL,
	[home_language] [nvarchar](50) NULL,
	[child_protection_plan] [bit] NULL,
	[child_in_need] [bit] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[students_insert](
	[school_id] [nvarchar](50) NULL,
	[academic_year] [nvarchar](50) NULL,
	[id] [nvarchar](50) NULL,
	[upi] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[initials] [nvarchar](50) NULL,
	[title] [nvarchar](max) NULL,
	[surname] [nvarchar](max) NULL,
	[forename] [nvarchar](max) NULL,
	[middle_names] [nvarchar](max) NULL,
	[legal_surname] [nvarchar](max) NULL,
	[legal_forename] [nvarchar](max) NULL,
	[gender] [nvarchar](50) NULL,
	[gender_identity] [nvarchar](50) NULL,
	[date_of_birth] [date] NULL,
	[restored_at] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[admission_number] [nvarchar](50) NULL,
	[upn] [nvarchar](50) NULL,
	[local_upn] [nvarchar](50) NULL,
	[former_upn] [nvarchar](50) NULL,
	[learner_number] [nvarchar](50) NULL,
	[current_nc_year] [nvarchar](50) NULL,
	[part_time] [nvarchar](50) NULL,
	[part_time_rate] [nvarchar](50) NULL,
	[admission_date] [date] NULL,
	[leaving_date] [date] NULL,
	[next_of_kin] [nvarchar](50) NULL,
	[religion] [nvarchar](50) NULL,
	[ethnicity] [nvarchar](50) NULL,
	[ethnicity_code] [nvarchar](50) NULL,
	[nationality] [nvarchar](50) NULL,
	[birth_place] [nvarchar](50) NULL,
	[custody_alert] [nvarchar](50) NULL,
	[custody_details] [nvarchar](50) NULL,
	[dietary_needs] [nvarchar](255) NULL,
	[paramedical_support] [nvarchar](50) NULL,
	[sen_status] [nvarchar](50) NULL,
	[first_language] [nvarchar](50) NULL,
	[marital_status] [nvarchar](50) NULL,
	[in_lea_care] [bit] NULL,
	[ever_in_care] [bit] NULL,
	[free_school_meals] [bit] NULL,
	[free_school_meals_6] [bit] NULL,
	[english_as_additional_language] [bit] NULL,
	[english_as_additional_language_status] [nvarchar](50) NULL,
	[premium_pupil_indicator] [bit] NULL,
	[premium_pupil_eligible] [bit] NULL,
	[premium_pupil_notes] [nvarchar](255) NULL,
	[service_children_indicator] [bit] NULL,
	[enrolment_status] [nvarchar](50) NULL,
	[enrolment_start_date] [date] NULL,
	[enrolment_end_date] [date] NULL,
	[gifted_and_talented_status] [bit] NULL,
	[national_insurance_number] [nvarchar](50) NULL,
	[application_status] [nvarchar](50) NULL,
	[boarding_status] [nvarchar](50) NULL,
	[permanent_resident] [nvarchar](50) NULL,
	[in_care_details] [nvarchar](50) NULL,
	[general_notes] [nvarchar](255) NULL,
	[leaver_destination] [nvarchar](255) NULL,
	[fsm_review_date] [nvarchar](50) NULL,
	[youth_support_services_agreement] [nvarchar](50) NULL,
	[responsible_care_authority] [nvarchar](50) NULL,
	[home_language] [nvarchar](50) NULL,
	[child_protection_plan] [bit] NULL,
	[child_in_need] [bit] NULL,
	[imported_at] [datetime] NULL
) 
GO
ALTER TABLE [dbo].[groups_students] ADD  DEFAULT (getdate()) FOR [imported_at]
GO
ALTER TABLE [dbo].[groups_students_insert] ADD  DEFAULT (getdate()) FOR [imported_at]
GO
ALTER TABLE [dbo].[student] ADD  DEFAULT (getdate()) FOR [imported_at]
GO
ALTER TABLE [dbo].[student_insert] ADD  DEFAULT (getdate()) FOR [imported_at]
GO
ALTER TABLE [dbo].[student_leaver] ADD  DEFAULT (getdate()) FOR [imported_at]
GO
ALTER TABLE [dbo].[student_leaver_insert] ADD  DEFAULT (getdate()) FOR [imported_at]
GO
ALTER TABLE [dbo].[students] ADD  DEFAULT (getdate()) FOR [imported_at]
GO
ALTER TABLE [dbo].[students_insert] ADD  DEFAULT (getdate()) FOR [imported_at]
GO

CREATE TABLE [dbo].[classes](
	[school_id] [nvarchar](50) NULL,
	[id] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[name] [nvarchar](255) NULL,
	[code] [nvarchar](255) NULL,
	[description] [nvarchar](255) NULL,
	[subject] [nvarchar](255) NULL,
	[alternative] [nvarchar](255) NULL,
	[priority] [nvarchar](50) NULL,
	[academic_year] [nvarchar](50) NULL,
	[year_group] [nvarchar](50) NULL,
	[restored_at] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[classes_employee](
	[school_id] [nvarchar](50) NULL,
	[class_id] [nvarchar](50) NULL,
	[employee_id] [nvarchar](50) NULL,
	[role] [nvarchar](255) NULL,
	[is_main_teacher] [bit] NULL,
	[is_class_teacher] [bit] NULL
) 
GO


CREATE TABLE [dbo].[classes_employee_insert](
	[school_id] [nvarchar](50) NULL,
	[class_id] [nvarchar](50) NULL,
	[employee_id] [nvarchar](50) NULL,
	[role] [nvarchar](255) NULL,
	[is_main_teacher] [bit] NULL,
	[is_class_teacher] [bit] NULL
) 
GO


CREATE TABLE [dbo].[classes_insert](
	[school_id] [nvarchar](50) NULL,
	[id] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[name] [nvarchar](255) NULL,
	[code] [nvarchar](255) NULL,
	[description] [nvarchar](255) NULL,
	[subject] [nvarchar](255) NULL,
	[alternative] [nvarchar](255) NULL,
	[priority] [nvarchar](50) NULL,
	[academic_year] [nvarchar](50) NULL,
	[year_group] [nvarchar](50) NULL,
	[restored_at] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[classes_student](
	[school_id] [nvarchar](50) NULL,
	[class_id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL
) 
GO


CREATE TABLE [dbo].[classes_student_insert](
	[school_id] [nvarchar](50) NULL,
	[class_id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL
) 
GO


CREATE TABLE [dbo].[subjects](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[code] [nvarchar](255) NULL,
	[name] [nvarchar](max) NULL,
	[active] [bit] NULL,
	[created_at] [smalldatetime] NULL,
	[updated_at] [smalldatetime] NULL,
	[imported_at] [smalldatetime] NULL
) 
GO
ALTER TABLE [dbo].[classes] ADD  DEFAULT (getdate()) FOR [imported_at]
GO
ALTER TABLE [dbo].[classes_insert] ADD  DEFAULT (getdate()) FOR [imported_at]
GO


CREATE TABLE [dbo].[school_details](
	[id] [nvarchar](max) NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[establishment_number] [int] NOT NULL,
	[urn] [int] NOT NULL,
	[phase_of_education] [nvarchar](max) NULL,
	[la_code] [int] NOT NULL,
	[timezone] [nvarchar](max) NULL,
	[mis] [nvarchar](max) NULL,
	[address_line_1] [nvarchar](max) NULL,
	[address_line_2] [nvarchar](max) NULL,
	[address_town] [nvarchar](max) NULL,
	[address_postcode] [nvarchar](max) NULL,
	[allows_writeback] [bit] NULL,
	[has_timetables] [bit] NULL,
	[has_lesson_attendance] [bit] NULL,
	[mat] [nvarchar](max) NULL
) 
GO




CREATE TABLE [dbo].[attendance_codes](
	[id] [nvarchar](50) NULL,
	[code] [nvarchar](50) NULL,
	[parent_code] [nvarchar](50) NULL,
	[gov_code] [nvarchar](50) NULL,
	[description] [nvarchar](250) NULL,
	[type] [nvarchar](250) NULL,
	[sub_type] [nvarchar](50) NULL
)
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A708867432', N'7', N'', N'', N'Illness due to Covid 19', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A973492091', N'7', N'', N'7', N'Illness due to Covid 19', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A541526275', N'8', N'', N'', N'Self-isolating due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A913933485', N'8', N'', N'8', N'Self-isolating due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1114701279', N'9', N'', N'9', N'Shielding due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A649833114', N'9', N'', N'', N'Shielding due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1634008453', N'-', N'', N'-', N'All should attend / No mark recorded', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1985777634', N'-', N'', N'', N'All should attend / No mark recorded', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A39307028', N'#', N'', N'', N'Planned whole or partial school closure', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A930902628', N'#', N'', N'#', N'Planned whole or partial school closure', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1329183376', N'/', N'', N'/', N'Present (AM)', N'PRESENT', N'Present')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A508116038', N'/', N'', N'', N'Present (AM)', N'PRESENT', N'Present')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1297912824', N';', N'', N';', N'Illness due to Covid 19', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1283217476', N'[', N'', N'[', N'Remote learning due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1161584171', N'\', N'', N'\', N'Present (PM)', N'PRESENT', N'Present')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A340644305', N'\', N'', N'', N'Present (PM)', N'PRESENT', N'Present')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1484231030', N']', N'', N']', N'Shielding due to Covid 19', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A813359890', N'=', N'', N'=', N'Illness due to Covid 19', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1019104821', N'B', N'', N'', N'Educated off site', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1529934786', N'B', N'', N'B', N'Educated off site', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1362597725', N'C', N'', N'C', N'Other authorised circumstances', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A850584524', N'C', N'', N'', N'Other authorised circumstances', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A742433741', N'C1', N'C', N'C1', N'Absent with leave for the purpose of participating in a regulated performance', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A951720191', N'C2', N'C', N'C2', N'Absent with leave, of compulsory school age and temporary reduced timetable does not require them to attend', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1219594087', N'D', N'', N'', N'Dual registration', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1462380788', N'D', N'', N'D', N'Dual registration', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1320442110', N'E', N'', N'', N'Excluded', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1831783567', N'E', N'', N'E', N'Excluded', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1151925385', N'F', N'', N'', N'Extended family holiday (agreed)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1663918118', N'F', N'', N'F', N'Extended family holiday (agreed)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1521455136', N'G', N'', N'', N'Family Holiday (not agreed)', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A2032141745', N'G', N'', N'G', N'Unauthorised holiday', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1352807867', N'H', N'', N'', N'Annual family holiday (agreed)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A2133121352', N'H', N'', N'H', N'Annual family holiday (agreed)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A2133121352', N'H', N'', N'', N'Annual family holiday (agreed)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1453771090', N'I', N'', N'', N'Illness', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1964735203', N'I', N'', N'I', N'Illness Not Medical or dental appointment', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A828725974', N'I01', N'I', N'I01', N'Illness', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A928644705', N'I02', N'I', N'I02', N'Illness- Confirmed case of coronavirus (COVID-19)', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1500139297', N'J', N'', N'J', N'Approved education activity as pupil is attending interview', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1822780141', N'J', N'', N'', N'Interview', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A575096676', N'J1', N'', N'J1', N'Absent with leave for the purpose of attending an interview for employment or for admission to another educational institution', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A374083122', N'K', N'', N'K', N'Attending a place, other than the school or another school at which they are a registered pupil, for educational provision arranged by LA', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1654132356', N'L', N'', N'', N'Late (before reg closed)', N'PRESENT', N'Late')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A186523258', N'L', N'', N'L', N'Late (before reg closed)', N'PRESENT', N'Late')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A18002453', N'M', N'', N'M', N'Medical / dental appointments', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A2023531039', N'M', N'', N'', N'Medical/Dental', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A118577068', N'N', N'', N'N', N'No reason yet provided for absence', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A2123581366', N'N', N'', N'', N'No reason yet provided for absence', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1955994433', N'O', N'', N'', N'Unauthorised Absence', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A487975751', N'O', N'', N'O', N'Unauthorised absence', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A176864472', N'P', N'', N'', N'Approved sporting activity', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A319327454', N'P', N'', N'P', N'Approved sporting activity', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1052687510', N'Q', N'', N'Q', N'Unable to attend because of lack of access arrangements by LA to facilitate their attendance', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A688857193', N'R', N'', N'R', N'Religious observance', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A8999027', N'R', N'', N'', N'Religious observance', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A109966346', N'S', N'', N'', N'Study leave', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A788775936', N'S', N'', N'S', N'Study leave', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A478185893', N'T', N'', N'', N'Traveller absence', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A621189531', N'T', N'', N'T', N'Traveller absence', N'AUTHORISED', N'Authorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A310848828', N'U', N'', N'', N'Late (after registers closed)', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A990199090', N'U', N'', N'U', N'Late (after registers closed)', N'UNAUTHORISED', N'Unauthorised Absence')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A687591127', N'V', N'', N'', N'Educational visit', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A830070477', N'V', N'', N'V', N'Educational visit or trip', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1601102008', N'W', N'', N'W', N'Approved education activity as pupil is attending work experience', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A788423278', N'W', N'', N'', N'Work Experience', N'PRESENT', N'Approved Educational Activity')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1131916182', N'X', N'X', N'X', N'Non-compulsory school age absence - not counted in possible attendances', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A206775165', N'X', N'X', N'', N'Non-compulsory school age or COVID-19 related', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A124965801', N'X01', N'X', N'X01', N'Non compulsory school age pupil not required to be in school', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A493975360', N'X02', N'X', N'X02', N'Pupil self-isolating with coronavirus (COVID-19) symptoms', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A326371547', N'X03', N'X', N'', N'Pupil self-isolating due to potential contact with a confirmed case of coronavirus (COVID-19) inside the school', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A694725746', N'X04', N'X', N'', N'Pupil self-isolating due to potential contact with a confirmed case of coronavirus (COVID-19) outside the school', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A795820045', N'X05', N'X', N'X05', N'Pupils required to self-isolate as part of quarantine requirement (after arriving in the UK from a non-exempt country or territory)', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A627303844', N'X06', N'X', N'X06', N'Pupil not in school because they have been advised specifically by their Doctor or public health authority that they are clinically extremely vulnerable and should not attend', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A996587839', N'X07', N'X', N'X07', N'Pupil advised specifically not to attend school as part of restrictions to education set out in Government advice', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A323305093', N'X08', N'X', N'X08', N'Pupil not attending school in line with advice from Directors of Public Health as part of outbreak management', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A700043804', N'X09', N'X', N'X09', N'Pupil or student in a specific group required, by NHS test and trace, to self-isolate as a close contact of a confirmed case of COVID-19 (e.g. those over 18 years 6 months and not fully vaccinated)', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1433240659', N'Y', N'', N'', N'Enforced closure - not counted in possible attendances', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1801476586', N'Y', N'', N'Y', N'Enforced closure - not counted in possible attendances', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A620562425', N'Y', N'', N'', N'Unable to attend due to exceptional circumstances', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A884822049', N'Y1', N'', N'Y1', N'Unable to attend because school is not within walking distance of pupil''s home and transport to and from school normally provided is not available', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1253176760', N'Y2', N'', N'Y2', N'Unable to attend due to widespread disruption to travel caused by a local, national, or international emergency', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1085573459', N'Y3', N'', N'Y3', N'Part of the school premises is unavoidably out of use and pupil cannot be accommodated in parts of the premises that remain in use', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1185622762', N'Y4', N'', N'Y4', N'Whole school closed when school was due to meet for a session, but session has been cancelled', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1555025541', N'Y5', N'', N'Y5', N'Unable to attend because pupil is subject to a sentence of detention', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1386373660', N'Y6', N'', N'Y6', N'Travel to or attendance at school would be contrary/prohibited by guidance/law relating to incidence or transmission of infection or disease', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1755383735', N'Y7', N'', N'Y7', N'Unable to attend because of any other unavoidable cause', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A1299388415', N'Z', N'', N'Z', N'Pupil not on roll', N'VOID', N'Attendance not required')
GO
INSERT [dbo].[attendance_codes] ([id], [code], [parent_code], [gov_code], [description], [type], [sub_type]) VALUES (N'A407541935', N'Z', N'', N'', N'DfES Z: Pupil not on roll', N'VOID', N'Attendance not required')
GO







CREATE FUNCTION [dbo].[get_acad_end] (@acad_year nvarchar(50))
RETURNS SMALLDATETIME
AS
  BEGIN
      DECLARE @Date AS SMALLDATETIME;

	  SET @Date = CASE 
                    WHEN @acad_year = '2023/2023' THEN '30 Jul 2023' 
                    WHEN @acad_year = '2023/2024' THEN '30 Jul 2024' 
                    WHEN @acad_year = '2024/2025' THEN '30 Jul 2025' 
                    WHEN @acad_year = '2025/2026' THEN '30 Jul 2026' 
                    WHEN @acad_year = '2026/2027' and GetDate() between '01 Sep 2026' and '30 Jul 2027' THEN Dateadd(day, -1, Getdate()) 
                    WHEN @acad_year = '2026/2027' THEN '30 Jul 2027'
                    WHEN @acad_year = '2027/2028' and GetDate() between '01 Sep 2027' and '30 Jul 2028' THEN Dateadd(day, -1, Getdate()) 
                    WHEN @acad_year = '2027/2028' THEN '30 Jul 2028'
                  END;
	  

      RETURN @Date;
  END

GO


CREATE FUNCTION [dbo].[get_acad_start] (@acad_year nvarchar(50))
RETURNS SMALLDATETIME
AS
  BEGIN
      DECLARE @Date AS SMALLDATETIME;

	  SET @Date = CASE 
                    WHEN @acad_year = '2022/2023' THEN '01 Sep 2022' 
                    WHEN @acad_year = '2023/2024' THEN '01 Sep 2023' 
                    WHEN @acad_year = '2024/2025' THEN '01 Sep 2024' 
                    WHEN @acad_year = '2025/2026' THEN '01 Sep 2025' 
                    WHEN @acad_year = '2026/2027' THEN '01 Sep 2026' 
                    WHEN @acad_year = '2027/2028' THEN '01 Sep 2027' 
                  END;
	  

      RETURN @Date;
  END
GO


CREATE FUNCTION [dbo].[get_year_group_clean]
(
	-- Add the parameters for the function here
	@year_group nvarchar(50)
)
RETURNS nvarchar(50)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @year_group_clean nvarchar(50)

	-- Add the T-SQL statements to compute the return value here
	SELECT @year_group_clean = 
	CASE 
	  WHEN @year_group in ('(1)', '(2)', '(3)', '(4)', '(5)', '(6)', '(7)', '(8)', '(9)', '( 1)', '( 2)', '( 3)', '( 4)', '( 5)', '( 6)', '( 7)', '( 8)', '( 9)', '(10)', '(11)','(12)','(13)') THEN 'Leaver'

	  WHEN @year_group in ('Year N1','YN1', 'N1') THEN 'N1'
	  WHEN @year_group in ('Year N2','YN2', 'N2') THEN 'N2'
	  WHEN @year_group in ('R', 'YR','Year R','Reception') THEN 'Reception'
	  WHEN @year_group in ('( 1)', '1','Y1','Year 1','Year Y1','Yr1','01','A1','1-2025') THEN '1'
	  WHEN @year_group in ('( 2)', '2','Y2','Year 2','Year Y2','Yr2','02','A2','2-2025') THEN '2'

	  WHEN @year_group in ('( 3)', '3','Y3','Year 3','Year Y3','Yr3','03','A3','3-2025') THEN '3'
	  WHEN @year_group in ('( 4)', '4','Y4','Year 4','Year Y4','Yr4','04','A4','4-2025') THEN '4'
	  WHEN @year_group in ('( 5)', '5','Y5','Year 5','Year Y5','Yr5','05','A5','5-2025') THEN '5'
	  WHEN @year_group in ('( 6)', '6','Y6','Year 6','Year Y6','Yr6','06','A6','6-2025') THEN '6'

	  WHEN @year_group in ('( 7)', '7','Y7','Year 7','Year Y7','Yr7','07','A7') THEN '7'
	  WHEN @year_group in ('( 8)', '8','Y8','Year 8','Year Y8','Yr8','08','A8') THEN '8'
	  WHEN @year_group in ('( 9)', '9','Y9','Year 9','Year Y9','Yr9','09','A9') THEN '9'

	  WHEN @year_group in ('(10)', '10','Y10','Year 10','Year Y10','Yr10','010','A10') THEN '10'
	  WHEN @year_group in ('(11)', '11','Y11','Year 11','Year Y11','Yr11','011','A11') THEN '11'

	  WHEN @year_group in ('(12)', '12','Year 12') THEN '12'
	  WHEN @year_group in ('(13)', '13','Year 13') THEN '13'

	  WHEN @year_group in ('No Year Group') THEN 'No Year Group'
	  else '#Unmapped#'+@year_group 
	end

	-- Return the result of the function
	RETURN @year_group_clean

END
GO

CREATE VIEW [dbo].[v_student_groups]
AS
  SELECT g.school_id,
         g.id          AS group_id,
         g.mis_id,
         g.name        AS group_name,
         g.code        AS group_code,
         g.type        AS group_type,
         g.description AS group_description,
         g.notes       AS group_notes,
         g.division    AS group_division,
         gs.student_id,
         gs.start_date,
         gs.end_date,
         gs.imported_at
  FROM   dbo.groups AS g
         INNER JOIN dbo.groups_students AS gs
                 ON g.id = gs.group_id

GO 



CREATE TABLE [dbo].[first_language_codes](
	[Code] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL
) 
GO







CREATE VIEW [dbo].[v_attendance_session]
AS
  SELECT att.school_id                              AS school_id_attend,
         att.student_id                             AS person_id,
         att_codes.code                             AS mark,
         att_codes.description,
         att_codes.sub_type                         AS statistical_description,
         att.date                                   AS session_start,
         att.date                                   AS session_start_date,
         Datename(dw, att.date) + ' ' + att.session AS event_session_description,
         CASE
           WHEN sub_type = 'Present' THEN 'att-colour-present'
           WHEN sub_type = 'Approved Educational Activity' THEN 'att-colour-aea'
           WHEN sub_type = 'Late' THEN 'att-colour-late'
           WHEN sub_type = 'Authorised Absence' THEN 'att-colour-aa'
           WHEN sub_type = 'Unauthorised Absence' THEN 'att-colour-ua'
           WHEN sub_type = 'Attendance not required' THEN 'att-colour-nr'
           WHEN sub_type = 'Missing' THEN 'att-colour-missing'
           ELSE ''
         END                                        AS class_name
  FROM   dbo.attendance_session AS att
         INNER JOIN dbo.attendance_codes AS att_codes
                 ON att_codes.id = att.attendance_code_id
  WHERE  ( att.date >= '01 Sep 2020' )

GO 





CREATE VIEW [dbo].[v_attendance_session_summary]
AS
SELECT att.School_id, att.student_id AS student_id, 
att.date AS session_start, 
SUM(CASE WHEN NOT type IN ('VOID', 'MISSING') THEN 1 ELSE 0 END) AS possible_sessions, 
SUM(CASE WHEN type = 'PRESENT' THEN 1 ELSE 0 END) AS attended_sessions, 
SUM(CASE WHEN type = 'PRESENT' AND NOT (sub_type IN ('Late', 'Approved Educational Activity')) THEN 1 ELSE 0 END) AS present_sessions, 
SUM(CASE WHEN type = 'AUTHORISED' THEN 1 ELSE 0 END) AS authorised_absence_sessions, 
SUM(CASE WHEN type = 'UNAUTHORISED' THEN 1 ELSE 0 END) AS unauthorised_absence_sessions, 
SUM(CASE WHEN code = 'G' THEN 1 ELSE 0 END) AS unauthorised_holiday_sessions, 
SUM(CASE WHEN sub_type = 'Approved Educational Activity' THEN 1 ELSE 0 END) AS approved_educational_activity_sessions, 
SUM(CASE WHEN sub_type = 'Late' THEN 1 ELSE 0 END) AS late_sessions, 
SUM(CASE WHEN type = 'MISSING' OR code = '-' THEN 1 ELSE 0 END) AS missing_sessions
FROM dbo.attendance_session AS att 
INNER JOIN dbo.attendance_codes AS att_codes ON att_codes.id = att.attendance_code_id
GROUP BY att.School_id, att.student_id, att.date
GO






CREATE VIEW [dbo].[v_behaviour]
AS
  SELECT dbo.behaviours_table.school_id,
         dbo.behaviours_table.id as W_id,
         dbo.behaviours_students.student_id,
         dbo.behaviours_table.behaviour_type,
         dbo.behaviours_table.location,
         dbo.behaviours_table.subject,
         dbo.behaviours_table.class,
         dbo.behaviours_table.status,
         dbo.behaviours_table.action,
         dbo.behaviours_table.comment,
         dbo.behaviours_table.parents_notified,
         dbo.behaviours_students.points_meta as points,
         dbo.behaviours_table.incident_date,
         dbo.behaviours_table.action_date,
         dbo.behaviours_table.created_on,
         dbo.behaviours_table.recorded_on,
         dbo.behaviours_table.updated_on,
         dbo.behaviours_table.imported_on
  FROM   dbo.behaviours_table
         INNER JOIN dbo.behaviours_students
                 ON dbo.behaviours_table.school_id = dbo.behaviours_students.school_id
                    AND dbo.behaviours_table.id = dbo.behaviours_students.id

GO




CREATE VIEW [dbo].[v_exclusions]
AS
  SELECT ex.school_id,
         ex.id,
         st.student_id,
         ex.type,
         ex.type_code,
         CASE WHEN Upper(ex.type) in ('SUSPENSION', 'FIXEDTERMPERIOD','FIXEDPERIODEXCLUSION') then 1 else 0 END as is_susp,
         CASE WHEN Upper(ex.type) in ('PERMANENT', 'PERM','PERMANENTEXCLUSION') then 1 else 0 END as is_perm,
         ex.reason,
         ex.reason_code,
         ex.start_date,
         ex.start_session,
         ex.end_date,
         ex.end_session,
         ex.sessions,
         ex.days,
         ex.academic_year,
         ex.term,
         ex.comments,
         ex.agencies_involved,
         ex.discipline_committee_date,
         ex.discipline_committee_result,
         ex.discipline_committee_reinstatement_date,
         ex.discipline_committee_representation_made,
         ex.appeal_received,
         ex.appeal_result,
         ex.appeal_result_date,
         ex.appeal_reinstatement_date,
         ex.created_at,
         ex.updated_at,
         ex.imported_at
  FROM   dbo.exclusion_table AS ex
         INNER JOIN dbo.exclusion_students AS st
                 ON ex.id = st.id

GO


CREATE TABLE [dbo].[student_sen_details](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[description] [nvarchar](max) NULL,
	[rank] [nvarchar](max) NULL,
	[category_code] [nvarchar](max) NULL,
	[category_description] [nvarchar](max) NULL,
	[type_code] [nvarchar](max) NULL,
	[type_description] [nvarchar](max) NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[imported_at] [datetime] NULL
)
GO

CREATE TABLE [dbo].[student_sen_details_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[description] [nvarchar](max) NULL,
	[rank] [nvarchar](max) NULL,
	[category_code] [nvarchar](max) NULL,
	[category_description] [nvarchar](max) NULL,
	[type_code] [nvarchar](max) NULL,
	[type_description] [nvarchar](max) NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[imported_at] [datetime] NULL
) 
GO



CREATE VIEW [dbo].[v_classes_student]
AS
select school_id, class_id, student_id
from classes_student
GO







CREATE VIEW [dbo].[v_student_subject_classes]
AS
  SELECT DISTINCT sd.name       AS school_name,
                  c.school_id,
                  c.id          AS class_id,
                  c.name        AS class_name,
                  c.description AS class_desc,
                  s.code        AS subject_code,
                  s.name        AS subject_name,
                  s.id          AS subject_id,
                  c.academic_year,
                  c.year_group,
                  cs.student_id
  FROM   dbo.classes AS c
         INNER JOIN dbo.subjects AS s
                 ON c.school_id = s.school_id
                    AND c.subject = s.id
         INNER JOIN dbo.classes_student cs on cs.class_id = c.id and cs.school_id = c.school_id
         INNER JOIN dbo.school_details AS sd
                 ON sd.id = c.school_id
GO



CREATE VIEW [dbo].[v_subject_classes]
AS
  SELECT DISTINCT sd.name                                AS school_name,
                  c.school_id,
                  c.id                                   AS class_id,
                  c.name                                 AS class_name,
                  c.description                          AS class_desc,
                  s.code                                 AS subject_code,
                  s.name                                 AS subject_name,
                  s.id                                   AS subject_id,
                  c.academic_year,
                  dbo.Get_year_group_clean(c.year_group) AS year_group
  FROM   dbo.classes AS c
         INNER JOIN dbo.subjects AS s
                 ON c.school_id = s.school_id
                    AND c.subject = s.id
         INNER JOIN dbo.school_details AS sd
                 ON sd.id = c.school_id
         INNER JOIN classes_student c_s
                 ON c_s.class_id = c.id
         INNER JOIN students st
                 ON st.id = c_s.student_id

GO 



CREATE TABLE [dbo].[calendar_days](
	[PKDate] [date] NOT NULL,
	[calendar_year] [smallint] NULL,
	[calendar_quarter] [tinyint] NULL,
	[calendar_quarter_desc] [varchar](10) NULL,
	[calendar_month] [tinyint] NULL,
	[calendar_month_name_long] [varchar](30) NULL,
	[calendar_month_name_short] [varchar](10) NULL,
	[calendar_week_in_year] [tinyint] NULL,
	[calendar_week_in_month] [tinyint] NULL,
	[calendar_max_week_in_month] [tinyint] NULL,
	[calendar_min_date_in_week] [date] NULL,
	[calendar_max_date_in_week] [date] NULL,
	[calendar_day_in_year] [smallint] NULL,
	[calendar_day_in_week] [tinyint] NULL,
	[calendar_day_in_month] [tinyint] NULL,
	[calendar_max_day_in_month] [tinyint] NULL,
	[calendar_min_date_in_month] [date] NULL,
	[calendar_max_date_in_month] [date] NULL,
	[Ordinal_day] [varchar](30) NULL,
	[dmy_name_long] [varchar](30) NULL,
	[dmy_name_long_with_suffix] [varchar](30) NULL,
	[day_name_long] [varchar](10) NULL,
	[day_name_short] [varchar](10) NULL,
	[continuous_year] [tinyint] NULL,
	[continuous_quarter] [smallint] NULL,
	[continuous_month] [smallint] NULL,
	[continuous_week] [smallint] NULL,
	[continuous_day] [int] NULL,
	[description] [varchar](100) NULL,
	[is_weekend] [tinyint] NULL,
	[is_holiday] [tinyint] NULL,
	[is_workday] [tinyint] NULL,
	[is_event] [tinyint] NULL,
PRIMARY KEY CLUSTERED 
(
	[PKDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO




CREATE TABLE [dbo].[attendance_session_student_breakdown_cache](
	[school_id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[attendance_pc] [decimal](9, 3) NULL,
	[attended_sessions] [int] NULL,
	[present_pc] [float] NULL,
	[present_sessions] [int] NULL,
	[approved_educational_activity_pc] [decimal](9, 3) NULL,
	[approved_educational_activity_sessions] [int] NULL,
	[absence_pc] [decimal](9, 3) NULL,
	[absence_sessions] [int] NULL,
	[authorised_absence_pc] [decimal](9, 3) NULL,
	[authorised_absence_sessions] [int] NULL,
	[unauthorised_absence_pc] [decimal](9, 3) NULL,
	[unauthorised_absence_sessions] [int] NULL,
	[unauthorised_holiday_pc] [decimal](9, 3) NULL,
	[unauthorised_holiday_sessions] [int] NULL,
	[late_pc] [decimal](9, 3) NULL,
	[late_sessions] [int] NULL,
	[missing_pc] [decimal](9, 3) NULL,
	[missing_sessions] [int] NULL,
	[possible_sessions] [int] NULL,
	[cached_at] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[attendance_session_student_breakdown_cache] ADD  DEFAULT (getdate()) FOR [cached_at]
GO




CREATE PROCEDURE [dbo].[attendance_session_student_breakdown_cache_process]
AS
BEGIN
    SELECT school_id, student_id,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Cast(Sum(attended_sessions) * 100 / Sum(possible_sessions) AS DECIMAL(9, 3))
       END                    AS attendance_pc,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Sum(attended_sessions)
       END                    AS attended_sessions,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Cast(Sum(present_sessions) * 100 / Sum(possible_sessions) AS DECIMAL(9, 3))
       END                    AS present_pc,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Sum(present_sessions)
       END                    AS present_sessions,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Cast(Sum(approved_educational_activity_sessions) * 100 / Sum(possible_sessions) AS DECIMAL(9, 3))
       END                    AS approved_educational_activity_pc,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Sum(approved_educational_activity_sessions)
       END                    AS approved_educational_activity_sessions,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Cast(( Sum(authorised_absence_sessions)
                     + Sum(unauthorised_absence_sessions) ) * 100 / Sum(possible_sessions) AS DECIMAL(9, 3))
       END                    AS absence_pc,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE( Sum(authorised_absence_sessions)
               + Sum(unauthorised_absence_sessions) )
       END                    AS absence_sessions,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Cast(Sum(authorised_absence_sessions) * 100 / Sum(possible_sessions) AS DECIMAL(9, 3))
       END                    AS authorised_absence_pc,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Sum(authorised_absence_sessions)
       END                    AS authorised_absence_sessions,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Cast(Sum(unauthorised_absence_sessions) * 100 / Sum(possible_sessions) AS DECIMAL(9, 3))
       END                    AS unauthorised_absence_pc,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Sum(unauthorised_absence_sessions)
       END                    AS unauthorised_absence_sessions,
       
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Cast(Sum(unauthorised_holiday_sessions) * 100 / Sum(possible_sessions) AS DECIMAL(9, 3))
       END                    AS unauthorised_holiday_pc,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Sum(unauthorised_holiday_sessions)
       END                    AS unauthorised_holiday_sessions,

       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Cast(Sum(late_sessions) * 100 / Sum(possible_sessions) AS DECIMAL(9, 3))
       END                    AS late_pc,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Sum(late_sessions)
       END                    AS late_sessions,
       CASE
         WHEN Sum(possible_sessions) = 0 THEN 0
         ELSE Cast(Sum(missing_sessions) * 100 / Sum(possible_sessions) AS DECIMAL(9, 3))
       END                    AS missing_pc,
       Sum(missing_sessions)  AS missing_sessions,
       Sum(possible_sessions) AS possible_sessions
    INTO #attendance_cache
    FROM   (SELECT school_id, student_id,
                   COALESCE(Cast(Sum(possible_sessions) AS FLOAT), 0)             AS possible_sessions,
                   COALESCE(Cast(Sum(attended_sessions) AS FLOAT), 0)             AS attended_sessions,
                   COALESCE(Cast(Sum(present_sessions) AS FLOAT), 0)              AS present_sessions,
                   COALESCE(Cast(Sum(authorised_absence_sessions) AS FLOAT), 0)   AS authorised_absence_sessions,
                   COALESCE(Cast(Sum(unauthorised_absence_sessions) AS FLOAT), 0) AS unauthorised_absence_sessions,
                   COALESCE(Cast(Sum(unauthorised_holiday_sessions) AS FLOAT), 0) AS unauthorised_holiday_sessions,
                   COALESCE(Cast(Sum(approved_educational_activity_sessions) AS FLOAT), 0)                  AS approved_educational_activity_sessions,
                   COALESCE(Cast(Sum(late_sessions) AS FLOAT), 0)                 AS late_sessions,
                   COALESCE(Cast(Sum(missing_sessions) AS FLOAT), 0)              AS missing_sessions
            FROM   (SELECT attsum.*
                    FROM   v_attendance_session_summary attsum
                           JOIN dbo.calendar_days c
                             ON c.pkdate = attsum.session_start
                    WHERE  session_start between '2026-09-01' and Getdate()) dt1
            GROUP  BY school_id, student_id)dt2
    GROUP  BY school_id, student_id 






    MERGE INTO attendance_session_student_breakdown_cache AS Target
    USING #attendance_cache AS Source
    ON Target.school_id = Source.school_id and Target.student_id = Source.student_id

    -- If match found → UPDATE
    WHEN MATCHED THEN
        UPDATE SET 
            Target.[attendance_pc] = Source.[attendance_pc],
              Target.[attended_sessions] = Source.[attended_sessions],
              Target.[present_pc] = Source.[present_pc],
              Target.[present_sessions] = Source.[present_sessions],
              Target.[approved_educational_activity_pc] = Source.[approved_educational_activity_pc],
              Target.[approved_educational_activity_sessions] = Source.[approved_educational_activity_sessions],
              Target.[absence_pc] = Source.[absence_pc],
              Target.[absence_sessions] = Source.[absence_sessions],
              Target.[authorised_absence_pc] = Source.[authorised_absence_pc],
              Target.[authorised_absence_sessions] = Source.[authorised_absence_sessions],
              Target.[unauthorised_absence_pc] = Source.[unauthorised_absence_pc],
              Target.[unauthorised_absence_sessions] = Source.[unauthorised_absence_sessions],
              Target.[unauthorised_holiday_pc] = Source.[unauthorised_holiday_pc],
              Target.[unauthorised_holiday_sessions] = Source.[unauthorised_holiday_sessions],
              Target.[late_pc] = Source.[late_pc],
              Target.[late_sessions] = Source.[late_sessions],
              Target.[missing_pc] = Source.[missing_pc],
              Target.[missing_sessions] = Source.[missing_sessions],
              Target.[possible_sessions] = Source.[possible_sessions]

    -- If no match → INSERT
    WHEN NOT MATCHED BY TARGET THEN
        INSERT ([school_id]
              ,[student_id]
              ,[attendance_pc]
              ,[attended_sessions]
              ,[present_pc]
              ,[present_sessions]
              ,[approved_educational_activity_pc]
              ,[approved_educational_activity_sessions]
              ,[absence_pc]
              ,[absence_sessions]
              ,[authorised_absence_pc]
              ,[authorised_absence_sessions]
              ,[unauthorised_absence_pc]
              ,[unauthorised_absence_sessions]
              ,[unauthorised_holiday_pc]
              ,[unauthorised_holiday_sessions]
              ,[late_pc]
              ,[late_sessions]
              ,[missing_pc]
              ,[missing_sessions]
              ,[possible_sessions]
              ,cached_at)
        VALUES (Source.school_id
              ,Source.[student_id]
              ,Source.[attendance_pc]
              ,Source.[attended_sessions]
              ,Source.[present_pc]
              ,Source.[present_sessions]
              ,Source.[approved_educational_activity_pc]
              ,Source.[approved_educational_activity_sessions]
              ,Source.[absence_pc]
              ,Source.[absence_sessions]
              ,Source.[authorised_absence_pc]
              ,Source.[authorised_absence_sessions]
              ,Source.[unauthorised_absence_pc]
              ,Source.[unauthorised_absence_sessions]
              ,Source.[unauthorised_holiday_pc]
              ,Source.[unauthorised_holiday_sessions]
              ,Source.[late_pc]
              ,Source.[late_sessions]
              ,Source.[missing_pc]
              ,Source.[missing_sessions]
              ,Source.[possible_sessions]
              ,GetDate());

        DROP TABLE #attendance_cache

END
GO



CREATE PROCEDURE [dbo].[Clean_exclusion_data]
AS
BEGIN


	UPDATE [dbo].[exclusion_table]
	SET academic_year = CASE 
	  WHEN start_date between '2028-09-01' and '2029-08-01' THEN '2028/2029'
	  WHEN start_date between '2027-09-01' and '2028-08-01' THEN '2027/2028'
	  WHEN start_date between '2026-09-01' and '2027-08-01' THEN '2026/2027'
	  WHEN start_date between '2025-09-01' and '2026-08-01' THEN '2025/2026'
	  WHEN start_date between '2024-09-01' and '2025-08-01' THEN '2024/2025'
	  WHEN start_date between '2023-09-01' and '2024-08-01' THEN '2023/2024'
	  WHEN start_date between '2022-09-01' and '2023-08-01' THEN '2022/2023'
	  WHEN start_date between '2021-09-01' and '2022-08-01' THEN '2021/2022'
	  ELSE NULL END
	Where academic_year is null


END
GO






CREATE FUNCTION [dbo].[tvf_students](@acad_year NVARCHAR(50) = '2026/2027')
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
	s.school_id,
	sch.name as school_name,
	sch.mat,
	s.id,
	s.upi,
	s.mis_id,
	s.initials,
	s.title,
	s.surname,
	s.forename,
	s.middle_names,
	s.legal_surname,
	s.legal_forename,
	CONCAT(COALESCE(s.legal_forename, ''), CASE WHEN s.legal_forename IS NULL OR s.legal_surname IS NULL THEN '' ELSE ' ' END, COALESCE(s.legal_surname, '')) AS display_name,
	s.gender,
	s.gender_identity,
	s.date_of_birth,
	s.restored_at,
	s.created_at,
	s.updated_at,
	s.admission_number,
	s.upn,
	s.local_upn,
	s.former_upn,
	s.learner_number,
	s.current_nc_year,
	s.part_time,
	s.part_time_rate,
	s.admission_date,
	s.leaving_date,
	s.next_of_kin,
	s.religion,
	s.ethnicity,
	s.ethnicity_code,
	s.nationality,
	s.birth_place,
	s.custody_alert,
	s.custody_details,
	s.dietary_needs,
	s.paramedical_support,
	s.sen_status,
	CASE 
	  WHEN sen_status = 'K' THEN 'SEN Support' 
	  WHEN sen_status = 'E' THEN 'Education, Health and Care Plan' 
	  WHEN sen_status = 'N' THEN 'No Special Educational Need'
	  ELSE ''
	END sen_status_description,
	s.first_language,
	f.Description as first_language_description,
	s.marital_status,
	s.in_lea_care,
	s.ever_in_care,
	s.free_school_meals,
	s.free_school_meals_6,
	s.english_as_additional_language,
	CASE 
	  WHEN english_as_additional_language = 0 THEN 'English is their first language' 
	  WHEN english_as_additional_language = 1 THEN 'English is not their first language' 
	  ELSE ''
	END english_as_additional_language_description,
	s.english_as_additional_language_status,
	s.premium_pupil_indicator,
	CASE 
	  WHEN premium_pupil_indicator = 0 THEN 'Is Pupil Premium Funded' 
	  WHEN premium_pupil_indicator = 1 THEN 'Is not Pupil Premium Funded' 
	  ELSE ''
	END premium_pupil_indicator_description,
	s.premium_pupil_eligible,
	s.premium_pupil_notes,
	s.service_children_indicator,
	CASE 
	  WHEN service_children_indicator = 0 THEN '' --No need to comment
	  WHEN service_children_indicator = 1 THEN 'Has parent in the service of the armed forces'
	  ELSE ''
	END service_children_indicator_description,
	s.enrolment_status,
	s.enrolment_start_date,
	s.enrolment_end_date,
	s.gifted_and_talented_status,
	s.national_insurance_number,
	s.application_status,
	s.boarding_status,
	s.permanent_resident,
	s.in_care_details,
	s.general_notes,
	s.leaver_destination,
	s.fsm_review_date,
	s.youth_support_services_agreement,
	s.responsible_care_authority,
	s.home_language,
	s.child_protection_plan,
	s.child_in_need,
	s.academic_year,
	'' as prior_attainment,
	s.imported_at,
	year_group_id,
	year_name,
	year_code,
	year_description,
	
	dbo.get_year_group_clean(Coalesce(year_name,current_nc_year,'No Year Group')) as year_group_clean,

	CASE 
	  WHEN dbo.get_year_group_clean(Coalesce(year_name,current_nc_year,'No Year Group')) in ('N1', 'N2', 'Reception') THEN 'KS0'
	  WHEN dbo.get_year_group_clean(Coalesce(year_name,current_nc_year,'No Year Group')) in ('1', '2') THEN 'KS1'
	  WHEN dbo.get_year_group_clean(Coalesce(year_name,current_nc_year,'No Year Group')) in ('3', '4', '5', '6') THEN 'KS2'
	  WHEN dbo.get_year_group_clean(Coalesce(year_name,current_nc_year,'No Year Group')) in ('7', '8', '9') THEN 'KS3'
	  WHEN dbo.get_year_group_clean(Coalesce(year_name,current_nc_year,'No Year Group')) in ('10', '11') THEN 'KS4'
	  WHEN dbo.get_year_group_clean(Coalesce(year_name,current_nc_year,'No Year Group')) in ('12', '13') THEN 'KS4'
	  WHEN Coalesce(year_name,current_nc_year,'No Year Group') in ('No Year Group') THEN ''
	  else '#Unmapped#' 
	end as year_group_phase,
	reg_group_id,
	reg_name,
	reg_code,
	reg_description,
	reg_employee_title,
	reg_employee_forename,
	reg_employee_surname,
	house_group_id,
	house_name,
	house_code,
	house_description,
	sen_category_code,
	sen_category_description,
	sen_type_code,
	sen_type_description,
	att.[attendance_pc],
    att.[attended_sessions],
    att.[present_pc],
    att.[present_sessions],
    att.[approved_educational_activity_pc],
    att.[approved_educational_activity_sessions],
    att.[absence_pc],
    att.[absence_sessions],
    att.[authorised_absence_pc],
    att.[authorised_absence_sessions],
    att.[unauthorised_absence_pc],
    att.[unauthorised_absence_sessions],
    att.[unauthorised_holiday_pc],
    att.[unauthorised_holiday_sessions],
    att.[late_pc],
    att.[late_sessions],
    att.[missing_pc],
    att.[missing_sessions],
    att.[possible_sessions]
	FROM dbo.students s
	JOIN school_details sch on sch.id=s.school_id
    LEFT JOIN attendance_session_student_breakdown_cache att on att.school_id = s.school_id and att.student_id = s.id
	LEFT JOIN first_language_codes f on f.Code = s.first_language
	OUTER APPLY(
		SELECT TOP 1 g.group_id as year_group_id, group_name as year_name, group_code as year_code, group_description as year_description
		FROM v_student_groups g
		Where g.student_id = s.id and g.school_id = s.school_id
		and (start_date is null or start_date < dbo.get_acad_start(@acad_year))
		and (end_date is null or end_date > dbo.get_acad_end(@acad_year))
		and group_type = 'YEAR'
		Order by g.imported_at desc
	) year_group
	OUTER APPLY(
		SELECT TOP 1 g.group_id as reg_group_id, group_name as reg_name, group_code as reg_code, group_description as reg_description, e.title as reg_employee_title, e.forename as reg_employee_forename, e.surname as reg_employee_surname
		FROM v_student_groups g
		LEFT JOIN groups_employees ge on ge.group_id = g.group_id
		LEFT JOIN employees e on e.id = ge.employee_id
		Where g.student_id = s.id and g.school_id = s.school_id
		and (start_date is null or start_date < dbo.get_acad_start(@acad_year))
		and (end_date is null or end_date > dbo.get_acad_end(@acad_year))
		and group_type = 'REGISTRATION'
		Order by g.imported_at desc
	) reg_group
	OUTER APPLY(
		SELECT TOP 1 g.group_id as house_group_id, group_name as house_name, group_code as house_code, group_description as house_description
		FROM v_student_groups g
		Where g.student_id = s.id and g.school_id = s.school_id
		and (start_date is null or start_date < dbo.get_acad_start(@acad_year))
		and (end_date is null or end_date > dbo.get_acad_end(@acad_year))
		and group_type = 'HOUSE'
		Order by g.imported_at desc
	) house_group
	OUTER APPLY(
		SELECT TOP 1 category_code as sen_category_code, category_description as sen_category_description, type_code as sen_type_code, type_description as sen_type_description
		FROM student_sen_details g
		Where g.student_id = s.id and g.school_id = s.school_id
		and (start_date is null or start_date < dbo.get_acad_start(@acad_year))
		and (end_date is null or end_date > dbo.get_acad_end(@acad_year))
		and rank = 1
		Order by g.imported_at desc
	) sen_need
	WHERE s.academic_year = @acad_year
	  AND (leaving_date is null OR leaving_date > dbo.get_acad_start(@acad_year))
)
GO



CREATE PROCEDURE [dbo].[school_details_Create](
@id nvarchar(50),
@name nvarchar(50),
@establishment_number int,
@urn int,
@phase_of_education nvarchar(50),
@la_code int,
@timezone nvarchar(50),
@mis nvarchar(50),
@address_line_1 nvarchar(50),
@address_line_2 nvarchar(50),
@address_town nvarchar(50),
@address_postcode nvarchar(50),
@allows_writeback bit,
@has_timetables bit,
@has_lesson_attendance bit)
AS
BEGIN
  DECLARE @rec_exist int;

  SELECT @rec_exist = count(*) 
  FROM school_details 
  WHERE id = @id --check if field already exists

  IF (@rec_exist <> 0) BEGIN
      UPDATE school_details
      SET name = @name,
		establishment_number = @establishment_number,
		urn = @urn,
		phase_of_education = @phase_of_education,
		la_code = @la_code,
		timezone = @timezone,
		mis = @mis,
		address_line_1 = @address_line_1,
		address_line_2 = @address_line_2,
		address_town = @address_town,
		address_postcode = @address_postcode,
		allows_writeback = @allows_writeback,
		has_timetables = @has_timetables,
		has_lesson_attendance = @has_lesson_attendance
      WHERE id = @id;
  END

	IF (@rec_exist = 0) BEGIN
        INSERT into school_details (id,
		name,
		establishment_number,
		urn,
		phase_of_education,
		la_code,
		timezone,
		mis,
		address_line_1,
		address_line_2,
		address_town,
		address_postcode,
		allows_writeback,
		has_timetables,
		has_lesson_attendance)
		values (@id,
		@name,
		@establishment_number,
		@urn,
		@phase_of_education,
		@la_code,
		@timezone,
		@mis,
		@address_line_1,
		@address_line_2,
		@address_town,
		@address_postcode,
		@allows_writeback,
		@has_timetables,
		@has_lesson_attendance);
	END
	RETURN 0
END

GO

