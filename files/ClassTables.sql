USE [GradeUp_Wonde_NENE]
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

ALTER TABLE [dbo].[classes] ADD  DEFAULT (getdate()) FOR [imported_at]
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

ALTER TABLE [dbo].[classes_insert] ADD  DEFAULT (getdate()) FOR [imported_at]
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

