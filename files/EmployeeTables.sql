
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
) ON [PRIMARY]
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
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[employee_email](
	[school_id] [nvarchar](50) NULL,
	[employee_id] [nvarchar](50) NULL,
	[email] [nvarchar](255) NULL,
	[primary] [nvarchar](255) NULL,
	[home] [nvarchar](255) NULL,
	[work] [nvarchar](255) NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[employee_email_insert](
	[school_id] [nvarchar](50) NULL,
	[employee_id] [nvarchar](50) NULL,
	[email] [nvarchar](255) NULL,
	[primary] [nvarchar](255) NULL,
	[home] [nvarchar](255) NULL,
	[work] [nvarchar](255) NULL
) ON [PRIMARY]
GO

