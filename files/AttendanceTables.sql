

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

