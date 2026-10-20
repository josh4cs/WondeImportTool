
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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[groups_students](
	[school_id] [nvarchar](50) NOT NULL,
	[student_id] [nvarchar](50) NULL,
	[group_id] [nvarchar](50) NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[imported_at] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[groups_students] ADD  DEFAULT (getdate()) FOR [imported_at]
GO


CREATE TABLE [dbo].[groups_students_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[student_id] [nvarchar](50) NULL,
	[group_id] [nvarchar](50) NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[imported_at] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[groups_students_insert] ADD  DEFAULT (getdate()) FOR [imported_at]
GO




CREATE TABLE [dbo].[groups_employees](
	[school_id] [nvarchar](50) NOT NULL,
	[employee_id] [nvarchar](50) NULL,
	[group_id] [nvarchar](50) NULL,
	[role] [nvarchar](50) NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[groups_employees_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[employee_id] [nvarchar](50) NULL,
	[group_id] [nvarchar](50) NULL,
	[role] [nvarchar](50) NULL
) ON [PRIMARY]
GO

