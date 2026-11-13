
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
--Example Data for IMPORT_LOG table
insert into dbo.IMPORT_LOG (school_id, request_id, table_name, instance, url, start_time, end_time, rows_processed) Values 
  ('A1836903598', 'c26cc3a4-c610-4e72-a58a-239e2e66fe59', 'students', 'request', 'https://api.wonde.com/v1.0/schools/A1836903598/students?per_page=500&include=extended_details,education_details,groups,sen_needs', '2026-10-21 09:11:31.727', '2026-10-21 09:11:31.727', 0),
  ('A1836903598', 'c26cc3a4-c610-4e72-a58a-239e2e66fe59', 'students', 'response', 'https://api.wonde.com/v1.0/schools/A1836903598/students?per_page=500&include=extended_details,education_details,groups,sen_needs', '2026-10-21 09:11:31.727', '2026-10-21 09:11:32.847', 0),
  ('A1836903598', 'c26cc3a4-c610-4e72-a58a-239e2e66fe59', 'students', 'insert batch complete', 'https://api.wonde.com/v1.0/schools/A1836903598/students?per_page=500&include=extended_details,education_details,groups,sen_needs', '2026-10-21 09:11:31.727', '2026-10-21 09:11:33.657', 200),
  ('A1836903598', 'c26cc3a4-c610-4e72-a58a-239e2e66fe59', 'students', 'request', 'https://api.wonde.com/v1.0/schools/A1836903598/students?per_page=200&include=extended_details%2Ceducation_details%2Cgroups%2Csen_needs&page=2', '2026-10-21 09:11:34.750', '2026-10-21 09:11:34.750', 0),
  ('A1836903598', 'c26cc3a4-c610-4e72-a58a-239e2e66fe59', 'students', 'response', 'https://api.wonde.com/v1.0/schools/A1836903598/students?per_page=200&include=extended_details%2Ceducation_details%2Cgroups%2Csen_needs&page=2', '2026-10-21 09:11:34.750', '2026-10-21 09:11:35.677', 0),
  ('A1836903598','c26cc3a4-c610-4e72-a58a-239e2e66fe59','students','insert batch complete','https://api.wonde.com/v1.0/schools/A1836903598/students?per_page=200&include=extended_details%2Ceducation_details%2Cgroups%2Csen_needs&page=2','2026-10-21 09:11:34.750','2026-10-21 09:11:36.247',200)