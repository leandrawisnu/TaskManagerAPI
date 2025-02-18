CREATE DATABASE [TaskManager]
GO
USE [TaskManager]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 29/12/2024 11:15:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Task]    Script Date: 29/12/2024 11:15:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Task](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[Title] [varchar](255) NOT NULL,
	[Description] [text] NOT NULL,
	[Status] [varchar](10) NOT NULL,
	[CreateAt] [datetime] NOT NULL,
	[DoneAt] [datetime] NULL,
 CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 29/12/2024 11:15:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varbinary](255) NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[Password] [varchar](255) NOT NULL,
	[RoleID] [int] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_User] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [FK_Task_Task] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Task_Task]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Role] ([ID])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Role]
GO
-- Insert dummy data into Role table
INSERT INTO [dbo].[Role] ([Name]) VALUES
('Admin'),
('Manager'),
('User');
GO
-- Insert dummy data into User table with MD5 hashed passwords
INSERT INTO [dbo].[User] ([Name], [Email], [Password], [RoleID]) VALUES
('Alice', 'alice@example.com', CONVERT(VARCHAR(255), HASHBYTES('MD5', 'password123'), 2), 1),
('Bob', 'bob@example.com', CONVERT(VARCHAR(255), HASHBYTES('MD5', '12345'), 2), 2),
('Charlie', 'charlie@example.com', CONVERT(VARCHAR(255), HASHBYTES('MD5', 'mypassword'), 2), 3),
('David', 'david@example.com', CONVERT(VARCHAR(255), HASHBYTES('MD5', 'securepass'), 2), 1),
('Eve', 'eve@example.com', CONVERT(VARCHAR(255), HASHBYTES('MD5', 'password2024'), 2), 2);
GO
-- Insert dummy data into Task table
INSERT INTO [dbo].[Task] ([UserID], [Title], [Description], [Status], [CreateAt], [DoneAt]) VALUES
(1, 'Setup Server', 'Setup the main production server.', 'Pending', GETDATE(), NULL),
(2, 'Develop Feature X', 'Implement and test feature X.', 'Progress', GETDATE(), NULL),
(3, 'Fix Bug Y', 'Debug and resolve issue Y in the system.', 'Completed', GETDATE(), DATEADD(DAY, -2, GETDATE())),
(NULL, 'Create Documentation', 'Write user manuals and technical documentation.', 'Pending', GETDATE(), NULL),
(4, 'Test New Release', 'Conduct testing for the latest release.', 'Progress', GETDATE(), NULL),
(NULL, 'Configure CI/CD', 'Set up CI/CD pipelines for automated deployment.', 'Pending', GETDATE(), NULL),
(5, 'Optimize Database', 'Optimize database queries for performance.', 'Progress', GETDATE(), NULL),
(NULL, 'Redesign Homepage', 'Redesign the company homepage for better UX.', 'Pending', GETDATE(), NULL),
(NULL, 'Develop Mobile App', 'Create a mobile version of the platform.', 'Pending', GETDATE(), NULL),
(1, 'Setup Monitoring', 'Implement monitoring tools for servers.', 'Completed', GETDATE(), DATEADD(DAY, -7, GETDATE())),
(2, 'Fix Login Issue', 'Resolve issues with the login functionality.', 'Completed', GETDATE(), DATEADD(DAY, -3, GETDATE())),
(NULL, 'Implement Caching', 'Add caching to reduce load times.', 'Pending', GETDATE(), NULL),
(3, 'Update API Docs', 'Update API documentation for developers.', 'Progress', GETDATE(), NULL),
(NULL, 'Test Security', 'Perform penetration testing on the system.', 'Pending', GETDATE(), NULL),
(NULL, 'Upgrade Framework', 'Upgrade to the latest framework version.', 'Pending', GETDATE(), NULL),
(4, 'Add Notifications', 'Add email and push notifications.', 'Progress', GETDATE(), NULL),
(5, 'Fix Payment Bug', 'Debug and resolve payment issues.', 'Completed', GETDATE(), DATEADD(DAY, -1, GETDATE())),
(NULL, 'Implement Dark Mode', 'Add a dark mode option for the UI.', 'Pending', GETDATE(), NULL),
(NULL, 'Improve SEO', 'Optimize the platform for search engines.', 'Pending', GETDATE(), NULL),
(1, 'Write Unit Tests', 'Increase test coverage with unit tests.', 'Progress', GETDATE(), NULL);
GO