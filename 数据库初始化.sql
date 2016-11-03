--创建PermissionInfo表（权限信息表）
CREATE TABLE [dbo].[PermissionInfo] (
    [PermissionId]          NVARCHAR (36) NOT NULL,
    [PermissionCode]        NVARCHAR (20) NOT NULL,
    [PermissionDescription] NVARCHAR (200) NULL,
    CONSTRAINT [PK_dbo.PermissionInfo] PRIMARY KEY CLUSTERED ([PermissionId] ASC)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [PermissionCodeIndex]
    ON [dbo].[PermissionInfo]([PermissionCode] ASC);
Go


--创建RoleInfo表（角色信息表）
CREATE TABLE [dbo].[RoleInfo] (
    [RoleId]          NVARCHAR (36) NOT NULL,
    [RoleName]        NVARCHAR (100) NULL,
    [RoleCode]        NVARCHAR (20) NOT NULL,
    [RoleDescription] NVARCHAR (200) NULL,
    CONSTRAINT [PK_dbo.RoleInfo] PRIMARY KEY CLUSTERED ([RoleId] ASC)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleCodeIndex]
    ON [dbo].[RoleInfo]([RoleCode] ASC);
Go

--创建RolePermission表（角色权限关系表）
CREATE TABLE [dbo].[RolePermissions] (
    [RoleId]       NVARCHAR (36) NOT NULL,
    [PermissionId] NVARCHAR (36) NOT NULL,
    CONSTRAINT [PK_dbo.RolePermissions] PRIMARY KEY CLUSTERED ([RoleId] ASC, [PermissionId] ASC),
    CONSTRAINT [FK_dbo.RolePermissions_dbo.RoleInfo_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[RoleInfo] ([RoleId]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_RoleId]
    ON [dbo].[RolePermissions]([RoleId] ASC);
Go

--创建UserInfo表（用户信息表）
CREATE TABLE [dbo].[UserInfo] (
    [Id]                   NVARCHAR (36) NOT NULL,
    [Email]                NVARCHAR (50) NULL,
    [UserName]             NVARCHAR (50) NOT NULL,
    [PasswordHash]         NVARCHAR (100) NULL,
    [Department]           NVARCHAR (50) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PhoneNumber]          NVARCHAR (20) NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    CONSTRAINT [PK_dbo.UserInfo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[UserInfo]([UserName] ASC);
Go


--创建UserRoles表（用户角色关系表）
CREATE TABLE [dbo].[UserRoles] (
    [UserId] NVARCHAR (36) NOT NULL,
    [RoleId] NVARCHAR (36) NOT NULL,
    CONSTRAINT [PK_dbo.UserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_dbo.UserRoles_dbo.RoleInfo_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[RoleInfo] ([RoleId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_RoleId]
    ON [dbo].[UserRoles]([RoleId] ASC);
Go





--添加管理员账户(用户名：Admin,密码：A123456789)
insert into UserInfo values('2383dd96-3e6c-45af-896d-ee69db71a793','AAAAA@qq.com','Admin','AAXop29zECFZ/hRQrti3Aa1oMVaPuPrzKbBYBOesFcdM/5AJunDRixQ12tmo/b+d3w==',NULL,0,NULL,0,0)
go

insert into RoleInfo values('029D8FA3-BD35-46E3-A63E-A3402E32E47E','管理员','Admin','系统管理的所有权限')
go

insert into UserRoles (UserId,RoleId) values('2383dd96-3e6c-45af-896d-ee69db71a793','029D8FA3-BD35-46E3-A63E-A3402E32E47E')
go


--创建ProjectInfo表（项目信息表）
CREATE TABLE [dbo].[ProjectInfo] (
    [Id]          NVARCHAR (36) NOT NULL,
    [ProjectName] NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_dbo.ProjectInfo] PRIMARY KEY CLUSTERED ([Id] ASC)
);
Go



--创建ResumeInfo（简历信息表）
CREATE TABLE [dbo].[ResumeInfo] (
    [Id]                  NVARCHAR (36) NOT NULL,
    [PersonalInformation] NVARCHAR (MAX) NULL,
    [CareerObjective]     NVARCHAR (MAX) NULL,
    [SelfAssessment]      NVARCHAR (MAX) NULL,
    [WorkExperience]      NVARCHAR (MAX) NULL,
    [ProjectExperience]   NVARCHAR (MAX) NULL,
    [Education]           NVARCHAR (MAX) NULL,
    [Certificates]        NVARCHAR (MAX) NULL,
    [HonorsandAwards]     NVARCHAR (MAX) NULL,
    [SchoolPractice]      NVARCHAR (MAX) NULL,
    [LanguageSkills]      NVARCHAR (MAX) NULL,
    [Training]            NVARCHAR (MAX) NULL,
    [ProfessionalSkills]  NVARCHAR (MAX) NULL,
    [FilePath]            NVARCHAR (200) NULL,
    [UploadTime]          DATETIME       NOT NULL,
    [SourceSite]          NVARCHAR (50) NULL,
    [LanguageType]        NVARCHAR (20) NULL,
    [ProjectName]         NVARCHAR (200) NULL,
    CONSTRAINT [PK_dbo.ResumeInfo] PRIMARY KEY CLUSTERED ([Id] ASC)
);
Go

--创建ResumeCommentInfo表（简历面试反馈信息表）
CREATE TABLE [dbo].[ResumeCommentInfo] (
    [Id]                NVARCHAR (36) NOT NULL,
    [ResumeId]          NVARCHAR (36) NULL,
    [PostName]          NVARCHAR (100) NULL,
    [InterviewDate]     DATETIME NULL,
    [InterviewFeedBack] NVARCHAR (MAX) NULL,
    [FeedBackTime]      DATETIME NULL,
    [UserName]          NVARCHAR (200) NULL,
    CONSTRAINT [PK_dbo.ResumeCommentInfo] PRIMARY KEY CLUSTERED ([Id] ASC)
);
Go
