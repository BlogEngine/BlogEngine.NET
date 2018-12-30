
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

alter table dbo.be_Users ADD comment varchar(max) null
GO

CREATE TABLE [dbo].[Contact](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RecordId] [varchar](255) NULL,
	[UserId] [varchar](255) NULL,
	[Information] [varchar](255) NULL,
	[Title] [varchar](255) NULL,
	[Name] [varchar](255) NULL,
	[UserName] [varchar](255) NULL,
	[DisplayName] [varchar](255) NULL,
	[FullName] [varchar](255) NULL,
	[FirstName] [varchar](255) NULL,
	[LastName] [varchar](255) NULL,
	[MiddleName] [varchar](255) NULL,
	[Spouse] [varchar](255) NULL,
	[AboutMe] [varchar](max) NULL,
	[PhotoUrl] [varchar](255) NULL,
	[Birthday] [varchar](50) NULL,
	[EmailAddress] [varchar](255) NULL,
	[Address] [varchar](255) NULL,
	[AddressAlt] [varchar](255) NULL,
	[CityTown] [varchar](255) NULL,
	[RegionState] [varchar](255) NULL,
	[Zip] [varchar](255) NULL,
	[Country] [varchar](50) NULL,
	[PhoneMain] [varchar](255) NULL,
	[WorkPhone] [varchar](255) NULL,
	[PhoneFax] [varchar](50) NULL,
	[PhoneMobile] [varchar](50) NULL,
	[Company] [varchar](255) NULL,
	[Organization] [varchar](255) NULL,
	[Website] [varchar](255) NULL,
	[Newsletter] [varchar](255) NULL,
	[Code] [varchar](255) NULL,
	[OtherName] [varchar](255) NULL,
	[Family] [varchar](255) NULL,
	[Benefactor] [varchar](255) NULL,
	[Gala] [varchar](255) NULL,
	[WineCheese] [varchar](255) NULL,
	[FundRaiser] [varchar](255) NULL,
	[Private] [bit] NULL,
	[Age] [varchar](200) NULL,
 CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

