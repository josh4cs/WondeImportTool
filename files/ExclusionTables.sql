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

