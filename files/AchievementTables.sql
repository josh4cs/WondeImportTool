CREATE TABLE [dbo].[achievements_students_insert] (
    [school_id]   VARCHAR (50) NOT NULL,
    [id]          VARCHAR (50) NULL,
    [student_id]  VARCHAR (50) NULL,
    [points]      INT          NULL,
    [points_meta] INT          NULL
);


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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[achievements_students] (
    [school_id]   VARCHAR (50) NOT NULL,
    [id]          VARCHAR (50) NULL,
    [student_id]  VARCHAR (50) NULL,
    [points]      INT          NULL,
    [points_meta] INT          NULL
);


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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO