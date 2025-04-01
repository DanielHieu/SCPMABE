/****** Object:  Table [dbo].[Area]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Area](
	[AreaId] [int] IDENTITY(1,1) NOT NULL,
	[ParkingLotId] [int] NOT NULL,
	[AreaName] [nvarchar](256) NOT NULL,
	[TotalFloor] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[RentalType] [int] NOT NULL,
 CONSTRAINT [PK_Area] PRIMARY KEY CLUSTERED 
(
	[AreaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssignedTask]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  Table [dbo].[Car]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Car](
	[CarId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Brand] [nvarchar](64) NULL,
	[Model] [nvarchar](64) NULL,
	[Color] [nvarchar](64) NULL,
	[LicensePlate] [nvarchar](64) NOT NULL,
	[RegistedDate] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
	[Thumbnail]    NVARCHAR (256) NULL
 CONSTRAINT [PK_Car] PRIMARY KEY CLUSTERED 
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Contract]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contract](
	[ContractId] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NOT NULL,
	[ParkingSpaceId] [int] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[Status] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[Note] [nvarchar](256) NULL,
 CONSTRAINT [PK_Contract] PRIMARY KEY CLUSTERED 
(
	[ContractId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[FirstName] [nvarchar](64) NOT NULL,
	[LastName] [nvarchar](64) NOT NULL,
	[Phone] [nvarchar](64) NOT NULL,
	[Email] [nvarchar](64) NULL,
	[Username] [nvarchar](64) NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Note] nvarchar(256) NULL,
	[PasswordTemp] nvarchar(128) NULL,
	[ActivationCode] nvarchar(32) NULL
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntryExitLog]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryExitLog](
	[EntryExitLogId] [int] IDENTITY(1,1) NOT NULL,
	[ParkingSpaceId] [int] NOT NULL,
	[LicensePlate] [nvarchar](64) NOT NULL,
	[PricePerHour] [decimal](18, 2) NOT NULL,
	[PricePerDay] [decimal](18, 2) NOT NULL,
	[PricePerMonth] [decimal](18, 2) NOT NULL,
	[EntryTime] [datetime] NOT NULL,
	[ExitTime] [datetime] NULL,
	[RentalType] [int] NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	IsPaid bit default(0) NOT NULL,
	EntranceImage nvarchar(255) NULL,
	ExitImage nvarchar(255) NULL
 CONSTRAINT [PK_EntryExitLog] PRIMARY KEY CLUSTERED 
(
	[EntryExitLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[FeedbackId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Message] [nvarchar](256) NOT NULL,
	[DateSubmitted] [datetime] NOT NULL,
	[Status] [int],
	[ResponsedContent] [nvarchar](256),
	[ResponsedAt] [dateTime]
 CONSTRAINT [PK_Feedback] PRIMARY KEY CLUSTERED 
(
	[FeedbackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Floor]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Floor](
	[FloorId] [int] IDENTITY(1,1) NOT NULL,
	[AreaId] [int] NOT NULL,
	[FloorName] [nvarchar](128) NOT NULL,
	[TotalParkingSpace] [int] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Floor] PRIMARY KEY CLUSTERED 
(
	[FloorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Owner]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Owner](
	[OwnerId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](64) NULL,
	[LastName] [nvarchar](64) NULL,
	[Phone] [nvarchar](64) NOT NULL,
	[Email] [nvarchar](64) NULL,
	[Username] [nvarchar](64) NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Owner] PRIMARY KEY CLUSTERED 
(
	[OwnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParkingLot]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingLot](
	[ParkingLotId] [int] IDENTITY(1,1) NOT NULL,
	[ParkingLotName] nvarchar(128) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[PricePerHour] [decimal](18, 2) NOT NULL,
	[PricePerDay] [decimal](18, 2) NOT NULL,
	[PricePerMonth] [decimal](18, 2) NOT NULL,
	[Address] [nvarchar](512) NOT NULL,
	[Long] [float] NULL,
	[Lat] [float] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ParkingLot] PRIMARY KEY CLUSTERED 
(
	[ParkingLotId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParkingLotPriceHistory]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingLotPriceHistory](
	[PriceLotPriceHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[ParkingLotID] [int] NOT NULL,
	[PricePerHour] [decimal](18, 2) NOT NULL,
	[PricePerDay] [decimal](18, 2) NOT NULL,
	[PricePerMonth] [decimal](18, 2) NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ParkingLotPriceHistory] PRIMARY KEY CLUSTERED 
(
	[PriceLotPriceHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParkingSpace]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingSpace](
	[ParkingSpaceId] [int] IDENTITY(1,1) NOT NULL,
	[FloorId] [int] NOT NULL,
	[ParkingSpaceName] [nvarchar](64) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_ParkingSpace] PRIMARY KEY CLUSTERED 
(
	[ParkingSpaceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParkingStatusSensor]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingStatusSensor](
	[ParkingStatusSensorId] [int] IDENTITY(1,1) NOT NULL,
	[ParkingSpaceId] [int] NOT NULL,
	[ApiKey] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ParkingStatusSensor] PRIMARY KEY CLUSTERED 
(
	[ParkingStatusSensorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentContract]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentContract](
	[PaymentContractId] [int] IDENTITY(1,1) NOT NULL,
	[ContractId] [int] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[PricePerMonth] [decimal](18, 2) NOT NULL,
	[PaymentMethod] [nvarchar](128) NULL,
	[PaymentDate] [datetime] NULL,
	[PaymentAmount] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[Note] [nvarchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PaymentContract] PRIMARY KEY CLUSTERED 
(
	[PaymentContractId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Staff]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Staff](
	[StaffId] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[FirstName] [nvarchar](64) NOT NULL,
	[LastName] [nvarchar](64) NOT NULL,
	[Phone] [nvarchar](64) NULL,
	[Email] [nvarchar](64) NOT NULL,
	[Username] [nvarchar](64) NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Staff] PRIMARY KEY CLUSTERED 
(
	[StaffId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskEach]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskEach](
	[TaskEachId] [int] IDENTITY(1,1) NOT NULL,
	[AssignedToId] [int] NOT NULL,
	[Title] nvarchar(128) NOT NULL,
	[Description] [nvarchar](256) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Priority] [int] NOT NULL,
	[Status] [int] NOT NULL
 CONSTRAINT [PK_TaskEach] PRIMARY KEY CLUSTERED 
(
	[TaskEachId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FCM]    Script Date: 3/23/2025 7:27:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FCM](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DeviceId] [nvarchar](128) NOT NULL,
	[DeviceToken] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_FCM] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Area] ON 

SET IDENTITY_INSERT [dbo].[Area] OFF
GO
SET IDENTITY_INSERT [dbo].[Car] ON 

SET IDENTITY_INSERT [dbo].[Car] OFF
GO
SET IDENTITY_INSERT [dbo].[Contract] ON 

SET IDENTITY_INSERT [dbo].[Contract] OFF
GO
SET IDENTITY_INSERT [dbo].[Customer] ON 

SET IDENTITY_INSERT [dbo].[Customer] OFF
GO
SET IDENTITY_INSERT [dbo].[EntryExitLog] ON 

SET IDENTITY_INSERT [dbo].[EntryExitLog] OFF
GO
SET IDENTITY_INSERT [dbo].[Feedback] ON 

SET IDENTITY_INSERT [dbo].[Feedback] OFF
GO
SET IDENTITY_INSERT [dbo].[Floor] ON 

SET IDENTITY_INSERT [dbo].[Floor] OFF
GO
SET IDENTITY_INSERT [dbo].[Owner] ON 

SET IDENTITY_INSERT [dbo].[Owner] OFF
GO
SET IDENTITY_INSERT [dbo].[ParkingLot] ON 

SET IDENTITY_INSERT [dbo].[ParkingLot] OFF
GO
SET IDENTITY_INSERT [dbo].[ParkingSpace] ON 

SET IDENTITY_INSERT [dbo].[ParkingSpace] OFF
GO
SET IDENTITY_INSERT [dbo].[Staff] ON 

SET IDENTITY_INSERT [dbo].[Staff] OFF
GO
SET IDENTITY_INSERT [dbo].[TaskEach] ON 

SET IDENTITY_INSERT [dbo].[TaskEach] OFF
GO
/****** Object:  Index [IX_ParkingStatusSensor]    Script Date: 3/23/2025 7:27:59 AM ******/
ALTER TABLE [dbo].[ParkingStatusSensor] ADD  CONSTRAINT [IX_ParkingStatusSensor] UNIQUE NONCLUSTERED 
(
	[ParkingStatusSensorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Car] ADD  CONSTRAINT [DF_Car_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Contract] ADD  CONSTRAINT [DF_Contract_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [DF_Customer_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Owner] ADD  CONSTRAINT [DF_Owner_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ParkingStatusSensor] ADD  CONSTRAINT [DF_ParkingStatusSensor_Status]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Staff] ADD  CONSTRAINT [DF_Staff_Active]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Area]  WITH CHECK ADD  CONSTRAINT [FK_Area_ParkingLot] FOREIGN KEY([ParkingLotId])
REFERENCES [dbo].[ParkingLot] ([ParkingLotId])
GO
ALTER TABLE [dbo].[Area] CHECK CONSTRAINT [FK_Area_ParkingLot]
GO
ALTER TABLE [dbo].[Car]  WITH CHECK ADD  CONSTRAINT [FK_Car_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[Car] CHECK CONSTRAINT [FK_Car_Customer]
GO
ALTER TABLE [dbo].[Contract]  WITH CHECK ADD  CONSTRAINT [FK_Contract_Car] FOREIGN KEY([CarId])
REFERENCES [dbo].[Car] ([CarId])
GO
ALTER TABLE [dbo].[Contract] CHECK CONSTRAINT [FK_Contract_Car]
GO
ALTER TABLE [dbo].[Contract]  WITH CHECK ADD  CONSTRAINT [FK_Contract_ParkingSpace] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpace] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[Contract] CHECK CONSTRAINT [FK_Contract_ParkingSpace]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_Owner] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[Owner] ([OwnerId])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_Owner]
GO
ALTER TABLE [dbo].[EntryExitLog]  WITH CHECK ADD  CONSTRAINT [FK_EntryExitLog_ParkingSpace] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpace] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[EntryExitLog] CHECK CONSTRAINT [FK_EntryExitLog_ParkingSpace]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Customer]
GO
ALTER TABLE [dbo].[Floor]  WITH CHECK ADD  CONSTRAINT [FK_Floor_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[Area] ([AreaId])
GO
ALTER TABLE [dbo].[Floor] CHECK CONSTRAINT [FK_Floor_Area]
GO
ALTER TABLE [dbo].[ParkingLot]  WITH CHECK ADD  CONSTRAINT [FK_ParkingLot_Owner] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[Owner] ([OwnerId])
GO
ALTER TABLE [dbo].[ParkingLot] CHECK CONSTRAINT [FK_ParkingLot_Owner]
GO
ALTER TABLE [dbo].[ParkingLotPriceHistory]  WITH CHECK ADD  CONSTRAINT [FK_ParkingLotPriceHistory_ParkingLot] FOREIGN KEY([ParkingLotID])
REFERENCES [dbo].[ParkingLot] ([ParkingLotId])
GO
ALTER TABLE [dbo].[ParkingLotPriceHistory] CHECK CONSTRAINT [FK_ParkingLotPriceHistory_ParkingLot]
GO
ALTER TABLE [dbo].[ParkingSpace]  WITH CHECK ADD  CONSTRAINT [FK_ParkingSpace_Floor] FOREIGN KEY([FloorId])
REFERENCES [dbo].[Floor] ([FloorId])
GO
ALTER TABLE [dbo].[ParkingSpace] CHECK CONSTRAINT [FK_ParkingSpace_Floor]
GO
ALTER TABLE [dbo].[ParkingStatusSensor]  WITH CHECK ADD  CONSTRAINT [FK_ParkingStatusSensor_ParkingSpace] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpace] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[ParkingStatusSensor] CHECK CONSTRAINT [FK_ParkingStatusSensor_ParkingSpace]
GO
ALTER TABLE [dbo].[PaymentContract]  WITH CHECK ADD  CONSTRAINT [FK_PaymentContract_Contract] FOREIGN KEY([ContractId])
REFERENCES [dbo].[Contract] ([ContractId])
GO
ALTER TABLE [dbo].[PaymentContract] CHECK CONSTRAINT [FK_PaymentContract_Contract]
GO
ALTER TABLE [dbo].[TaskEach]  WITH CHECK ADD  CONSTRAINT [FK_TaskEach_AssignedToId] FOREIGN KEY([AssignedToId])
REFERENCES [dbo].Staff ([StaffId])
GO
ALTER TABLE [dbo].[FCM]  WITH CHECK ADD  CONSTRAINT [FK_FCM_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[FCM] CHECK CONSTRAINT [FK_FCM_Customer]
GO