
CREATE TABLE [dbo].[contact_address_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[ID] [nvarchar](50) NULL,
	[address_type] [nvarchar](50) NULL,
	[house_number] [nvarchar](max) NULL,
	[house_name] [nvarchar](max) NULL,
	[apartment] [nvarchar](max) NULL,
	[street] [nvarchar](max) NULL,
	[district] [nvarchar](max) NULL,
	[town] [nvarchar](max) NULL,
	[county] [nvarchar](max) NULL,
	[country] [nvarchar](max) NULL,
	[postcode] [nvarchar](50) NULL,
	[data_imported_on] [datetime] NULL
) 
GO



CREATE TABLE [dbo].[contact_email_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[ID] [nvarchar](50) NULL,
	[Email] [nvarchar](500) NULL,
	[primary] [nvarchar](500) NULL,
	[home] [nvarchar](500) NULL,
	[work] [nvarchar](500) NULL,
	[data_imported_on] [datetime] NULL
)
GO


CREATE TABLE [dbo].[contact_phone_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[ID] [nvarchar](50) NULL,
	[Phone] [nvarchar](50) NULL,
	[primary] [nvarchar](50) NULL,
	[home] [nvarchar](50) NULL,
	[work] [nvarchar](50) NULL,
	[mobile] [nvarchar](50) NULL,
	[data_imported_on] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[contact_relationship_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[Contact_ID] [nvarchar](50) NULL,
	[Student_ID] [nvarchar](50) NULL,
	[bill_payer] [bit] NULL,
	[lives_with_pupil] [bit] NULL,
	[email_bills] [bit] NULL,
	[copy_bills] [bit] NULL,
	[court_order] [bit] NULL,
	[pupil_report] [bit] NULL,
	[parental_responsibility] [bit] NULL,
	[correspondence] [bit] NULL,
	[email_notification] [bit] NULL,
	[sms_notification] [bit] NULL,
	[mail_notification] [bit] NULL,
	[push_notification] [bit] NULL,
	[in_touch_communication] [bit] NULL,
	[priority] [int] NULL,
	[relationship] [nvarchar](50) NULL,
	[emergency_contact] [bit] NULL,
	[portal_access] [bit] NULL,
	[attendance_notification] [bit] NULL,
	[mail_merge] [bit] NULL,
	[created_at] [smalldatetime] NULL,
	[data_updated_on] [smalldatetime] NULL,
	[data_imported_on] [smalldatetime] NULL
) 
GO



CREATE TABLE [dbo].[contacts_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[ID] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[upi] [nvarchar](100) NULL,
	[title] [varchar](50) NULL,
	[initials] [varchar](20) NULL,
	[surname] [nvarchar](max) NULL,
	[forename] [nvarchar](max) NULL,
	[middle_name] [nvarchar](max) NULL,
	[gender] [varchar](20) NULL,
	[dob] [date] NULL,
	[created_at] [smalldatetime] NULL,
	[data_updated_on] [smalldatetime] NULL,
	[data_imported_on] [smalldatetime] NULL
) 
GO







CREATE TABLE [dbo].[contact_address](
	[school_id] [nvarchar](50) NOT NULL,
	[ID] [nvarchar](50) NULL,
	[address_type] [nvarchar](50) NULL,
	[house_number] [nvarchar](max) NULL,
	[house_name] [nvarchar](max) NULL,
	[apartment] [nvarchar](max) NULL,
	[street] [nvarchar](max) NULL,
	[district] [nvarchar](max) NULL,
	[town] [nvarchar](max) NULL,
	[county] [nvarchar](max) NULL,
	[country] [nvarchar](max) NULL,
	[postcode] [nvarchar](50) NULL,
	[data_imported_on] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[contact_email](
	[school_id] [nvarchar](50) NOT NULL,
	[ID] [nvarchar](50) NULL,
	[Email] [nvarchar](500) NULL,
	[primary] [nvarchar](500) NULL,
	[home] [nvarchar](500) NULL,
	[work] [nvarchar](500) NULL,
	[data_imported_on] [datetime] NULL
)
GO


CREATE TABLE [dbo].[contact_phone](
	[school_id] [nvarchar](50) NOT NULL,
	[ID] [nvarchar](50) NULL,
	[Phone] [nvarchar](50) NULL,
	[primary] [nvarchar](50) NULL,
	[home] [nvarchar](50) NULL,
	[work] [nvarchar](50) NULL,
	[mobile] [nvarchar](50) NULL,
	[data_imported_on] [datetime] NULL
) 
GO


CREATE TABLE [dbo].[contact_relationship](
	[school_id] [nvarchar](50) NOT NULL,
	[Contact_ID] [nvarchar](50) NULL,
	[Student_ID] [nvarchar](50) NULL,
	[bill_payer] [bit] NULL,
	[lives_with_pupil] [bit] NULL,
	[email_bills] [bit] NULL,
	[copy_bills] [bit] NULL,
	[court_order] [bit] NULL,
	[pupil_report] [bit] NULL,
	[parental_responsibility] [bit] NULL,
	[correspondence] [bit] NULL,
	[email_notification] [bit] NULL,
	[sms_notification] [bit] NULL,
	[mail_notification] [bit] NULL,
	[push_notification] [bit] NULL,
	[in_touch_communication] [bit] NULL,
	[priority] [int] NULL,
	[relationship] [nvarchar](50) NULL,
	[emergency_contact] [bit] NULL,
	[portal_access] [bit] NULL,
	[attendance_notification] [bit] NULL,
	[mail_merge] [bit] NULL,
	[created_at] [smalldatetime] NULL,
	[data_updated_on] [smalldatetime] NULL,
	[data_imported_on] [smalldatetime] NULL
) 
GO



CREATE TABLE [dbo].[contacts](
	[school_id] [nvarchar](50) NOT NULL,
	[ID] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[upi] [nvarchar](100) NULL,
	[title] [varchar](50) NULL,
	[initials] [varchar](20) NULL,
	[surname] [nvarchar](max) NULL,
	[forename] [nvarchar](max) NULL,
	[middle_name] [nvarchar](max) NULL,
	[gender] [varchar](20) NULL,
	[dob] [date] NULL,
	[created_at] [smalldatetime] NULL,
	[data_updated_on] [smalldatetime] NULL,
	[data_imported_on] [smalldatetime] NULL
) 
GO

