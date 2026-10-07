CREATE TABLE [dbo].[behaviours_students_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[points] [int] NULL,
	[points_meta] [int] NULL
) ON [PRIMARY]
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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[behaviours_students](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[student_id] [nvarchar](50) NULL,
	[points] [int] NULL,
	[points_meta] [int] NULL
) ON [PRIMARY]
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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO