
CREATE TABLE [dbo].[subjects](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[code] [nvarchar](255) NULL,
	[name] [nvarchar](max) NULL,
	[active] [bit] NULL,
	[created_at] [smalldatetime] NULL,
	[updated_at] [smalldatetime] NULL,
	[imported_at] [smalldatetime] NULL
) 
GO


CREATE TABLE [dbo].[subjects_insert](
	[school_id] [nvarchar](50) NOT NULL,
	[id] [nvarchar](50) NULL,
	[mis_id] [nvarchar](50) NULL,
	[code] [nvarchar](255) NULL,
	[name] [nvarchar](max) NULL,
	[active] [bit] NULL,
	[created_at] [smalldatetime] NULL,
	[updated_at] [smalldatetime] NULL,
	[imported_at] [smalldatetime] NULL
) 
GO
