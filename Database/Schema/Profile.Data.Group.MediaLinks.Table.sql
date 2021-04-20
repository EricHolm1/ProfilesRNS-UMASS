SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Profile.Data].[Group.MediaLinks](
	[UrlID] [varchar](50) NOT NULL,
	[GroupID] [int] NOT NULL,
	[URL] [varchar](max) NOT NULL,
	[WebPageTitle] [varchar](max) NOT NULL,
	[PublicationDate] [varchar](100) NULL,
	[SortOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UrlID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
