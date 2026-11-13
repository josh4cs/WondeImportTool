

CREATE TABLE [dbo].[bromcom_behaviour_event](
	[student_id] [int] NULL,
	[event_record_id] [int] NULL,
	[event_date] [datetime] NULL,
	[event_name] [nvarchar](max) NULL,
	[event_description] [nvarchar](max) NULL,
	[adjustment] [int] NULL,
	[collection_name] [nvarchar](100) NULL,
	[comment] [nvarchar](max) NULL,
	[internal_comment] [nvarchar](max) NULL,
	[day_of_week] [nvarchar](50) NULL,
	[event_type] [nvarchar](50) NULL,
	[staff_code] [nvarchar](50) NULL,
	[subject] [nvarchar](50) NULL,
	[department] [nvarchar](100) NULL,
	[room] [nvarchar](50) NULL,
	[timetable_period] [nvarchar](50) NULL
) 
GO


CREATE TABLE [dbo].[bromcom_behaviour_event_insert](
	[student_id] [int] NULL,
	[event_record_id] [int] NULL,
	[event_date] [datetime] NULL,
	[event_name] [nvarchar](max) NULL,
	[event_description] [nvarchar](max) NULL,
	[adjustment] [int] NULL,
	[collection_name] [nvarchar](100) NULL,
	[comment] [nvarchar](max) NULL,
	[internal_comment] [nvarchar](max) NULL,
	[day_of_week] [nvarchar](50) NULL,
	[event_type] [nvarchar](50) NULL,
	[staff_code] [nvarchar](50) NULL,
	[subject] [nvarchar](50) NULL,
	[department] [nvarchar](100) NULL,
	[room] [nvarchar](50) NULL,
	[timetable_period] [nvarchar](50) NULL
)
GO

