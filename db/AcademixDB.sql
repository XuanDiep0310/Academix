`/* ============================================================
   HỆ THỐNG ACADEMIX DATABASE - PRODUCTION READY
   - Sử dụng UTC cho tất cả timestamp (SYSUTCDATETIME())
   - Collation Vietnamese_CI_AS cho tiếng Việt
   - Indexes tối ưu cho performance
   - Constraints đầy đủ cho data integrity
   ============================================================ */

-- Tạo database với collation hỗ trợ tiếng Việt
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'AcademixDB')
BEGIN
    CREATE DATABASE AcademixDB
    COLLATE Vietnamese_CI_AS;
END
GO

USE AcademixDB;
GO

/* ============================================================
   1) USERS & ORGANIZATION
   ============================================================ */
CREATE TABLE dbo.Organization (
    OrganizationId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(250) NOT NULL,
    Domain NVARCHAR(200) NULL,
    LogoUrl NVARCHAR(1000) NULL,
    ThemeJson NVARCHAR(MAX) NULL,
    BillingContact NVARCHAR(200) NULL,

    -- UTC timestamps
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT CK_Organization_Domain CHECK (Domain IS NULL OR Domain LIKE '%.%')
);

CREATE TABLE dbo.[User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),

    -- Authentication
    Email NVARCHAR(256) NOT NULL,
    NormalizedEmail NVARCHAR(256) NOT NULL,
    UserName NVARCHAR(100) NULL,
    PasswordHash VARBINARY(MAX) NULL,
    PasswordSalt VARBINARY(MAX) NULL,
    IsEmailConfirmed BIT NOT NULL DEFAULT 0,

    -- 2FA
    TwoFAEnabled BIT NOT NULL DEFAULT 0,
    TwoFASecret NVARCHAR(512) NULL,

    -- Profile
    DisplayName NVARCHAR(200) NULL,
    Phone NVARCHAR(50) NULL,
    AvatarUrl NVARCHAR(1000) NULL,
    Bio NVARCHAR(1000) NULL,

    -- Status & Audit (ALL UTC)
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy INT NULL,
    UpdatedAt DATETIME2(7) NULL,
    UpdatedBy INT NULL,
    LastLoginAt DATETIME2(7) NULL,  -- UTC: convert to local when display

    CONSTRAINT CK_User_Email CHECK (Email LIKE '%@%.%'),
    CONSTRAINT UQ_User_Email UNIQUE (OrganizationId, NormalizedEmail)
);

CREATE INDEX IX_User_Email ON dbo.[User](OrganizationId, NormalizedEmail);
CREATE INDEX IX_User_Active ON dbo.[User](IsActive, IsDeleted) WHERE IsActive = 1 AND IsDeleted = 0;

-- Role & Permission
CREATE TABLE dbo.Role (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_Role_Name UNIQUE (OrganizationId, Name)
);

CREATE TABLE dbo.UserRole (
    UserRoleId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId) ON DELETE CASCADE,
    RoleId INT NOT NULL FOREIGN KEY REFERENCES dbo.Role(RoleId) ON DELETE CASCADE,
    AssignedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    AssignedBy INT NULL,

    CONSTRAINT UQ_UserRole UNIQUE (UserId, RoleId)
);

CREATE INDEX IX_UserRole_User ON dbo.UserRole(UserId);
CREATE INDEX IX_UserRole_Role ON dbo.UserRole(RoleId);

CREATE TABLE dbo.Permission (
    PermissionId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Code NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL
);

CREATE TABLE dbo.RolePermission (
    RolePermissionId INT IDENTITY(1,1) PRIMARY KEY,
    RoleId INT NOT NULL FOREIGN KEY REFERENCES dbo.Role(RoleId) ON DELETE CASCADE,
    PermissionId INT NOT NULL FOREIGN KEY REFERENCES dbo.Permission(PermissionId) ON DELETE CASCADE,

    CONSTRAINT UQ_RolePermission UNIQUE (RoleId, PermissionId)
);

CREATE INDEX IX_RolePermission_Role ON dbo.RolePermission(RoleId);

-- External OAuth login
CREATE TABLE dbo.ExternalLogin (
    ExternalLoginId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId) ON DELETE CASCADE,
    Provider NVARCHAR(100) NOT NULL,
    ProviderKey NVARCHAR(200) NOT NULL,
    DisplayName NVARCHAR(200) NULL,
    ConnectedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_ExternalLogin UNIQUE (Provider, ProviderKey)
);

CREATE INDEX IX_ExternalLogin_User ON dbo.ExternalLogin(UserId);

/* ============================================================
   2) COURSE / CLASS / ENROLLMENT
   ============================================================ */
CREATE TABLE dbo.Course (
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    Code NVARCHAR(50) NULL,
    Title NVARCHAR(300) NOT NULL,
    ShortDescription NVARCHAR(1000) NULL,
    LongDescription NVARCHAR(MAX) NULL,
    CreatedBy INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    IsPublished BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_Course_Code UNIQUE (OrganizationId, Code)
);

CREATE INDEX IX_Course_Published ON dbo.Course(IsPublished) WHERE IsPublished = 1;

CREATE TABLE dbo.Class (
    ClassId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL FOREIGN KEY REFERENCES dbo.Course(CourseId),
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    Title NVARCHAR(300) NOT NULL,
    TeacherId INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),

    -- Dates (lưu date, không timezone-sensitive)
    StartDate DATE NULL,
    EndDate DATE NULL,

    EnrollmentCode NVARCHAR(50) NULL,
    MaxStudents INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_Class_Dates CHECK (EndDate IS NULL OR EndDate >= StartDate),
    CONSTRAINT CK_Class_MaxStudents CHECK (MaxStudents IS NULL OR MaxStudents > 0)
);

CREATE INDEX IX_Class_Course ON dbo.Class(CourseId);
CREATE INDEX IX_Class_Teacher ON dbo.Class(TeacherId);
CREATE INDEX IX_Class_Active ON dbo.Class(IsActive) WHERE IsActive = 1;

CREATE TABLE dbo.Enrollment (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    ClassId INT NOT NULL FOREIGN KEY REFERENCES dbo.Class(ClassId),
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    RoleInClass NVARCHAR(50) NOT NULL DEFAULT 'Student',

    -- UTC timestamps
    JoinedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    LeftAt DATETIME2(7) NULL,

    IsApproved BIT NOT NULL DEFAULT 1,
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT UQ_Enrollment UNIQUE (ClassId, UserId),
    CONSTRAINT CK_Enrollment_Role CHECK (RoleInClass IN ('Student', 'TA'))
);

CREATE INDEX IX_Enrollment_Class ON dbo.Enrollment(ClassId);
CREATE INDEX IX_Enrollment_User ON dbo.Enrollment(UserId);
CREATE INDEX IX_Enrollment_Active ON dbo.Enrollment(ClassId, UserId, IsActive);

/* ============================================================
   3) FILE STORAGE & RESOURCE
   ============================================================ */
CREATE TABLE dbo.FileStorage (
    FileId INT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    UploadedBy INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    FileName NVARCHAR(500) NOT NULL,
    BlobUri NVARCHAR(2000) NULL,
    StorageProvider NVARCHAR(100) NULL,
    ContentType NVARCHAR(200) NULL,
    ContentLength BIGINT NULL,
    Checksum NVARCHAR(128) NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_FileStorage_Size CHECK (ContentLength IS NULL OR ContentLength >= 0)
);

CREATE INDEX IX_FileStorage_Org ON dbo.FileStorage(OrganizationId);
CREATE INDEX IX_FileStorage_Uploaded ON dbo.FileStorage(UploadedBy);

CREATE TABLE dbo.Resource (
    ResourceId INT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    CourseId INT NULL FOREIGN KEY REFERENCES dbo.Course(CourseId),
    ClassId INT NULL FOREIGN KEY REFERENCES dbo.Class(ClassId),
    Title NVARCHAR(500) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ResourceType NVARCHAR(50) NOT NULL,
    CurrentVersionId INT NULL,
    Visibility NVARCHAR(50) NOT NULL DEFAULT 'Public',
    CreatedBy INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT CK_Resource_Type CHECK (ResourceType IN ('Document', 'Video', 'Link', 'Audio', 'Other')),
    CONSTRAINT CK_Resource_Visibility CHECK (Visibility IN ('Public', 'Private', 'ClassOnly'))
);

CREATE INDEX IX_Resource_Course ON dbo.Resource(CourseId);
CREATE INDEX IX_Resource_Class ON dbo.Resource(ClassId);
CREATE INDEX IX_Resource_Active ON dbo.Resource(IsDeleted) WHERE IsDeleted = 0;

CREATE TABLE dbo.ResourceVersion (
    ResourceVersionId INT IDENTITY(1,1) PRIMARY KEY,
    ResourceId INT NOT NULL FOREIGN KEY REFERENCES dbo.Resource(ResourceId) ON DELETE CASCADE,
    FileId INT NULL FOREIGN KEY REFERENCES dbo.FileStorage(FileId),
    VersionNumber INT NOT NULL DEFAULT 1,
    Notes NVARCHAR(1000) NULL,
    DurationSeconds INT NULL,
    SizeBytes BIGINT NULL,
    CreatedBy INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_ResourceVersion_Number CHECK (VersionNumber > 0),
    CONSTRAINT UQ_ResourceVersion UNIQUE (ResourceId, VersionNumber)
);

CREATE INDEX IX_ResourceVersion_Resource ON dbo.ResourceVersion(ResourceId);

/* ============================================================
   4) COMMENT / LIKE (GENERIC)
   ============================================================ */
CREATE TABLE dbo.Comment (
    CommentId INT IDENTITY(1,1) PRIMARY KEY,
    EntityType NVARCHAR(100) NOT NULL,
    EntityId INT NOT NULL,
    ParentCommentId INT NULL FOREIGN KEY REFERENCES dbo.Comment(CommentId),
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    Content NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT CK_Comment_Content CHECK (LEN(Content) > 0)
);

CREATE INDEX IX_Comment_Entity ON dbo.Comment(EntityType, EntityId);
CREATE INDEX IX_Comment_User ON dbo.Comment(UserId);
CREATE INDEX IX_Comment_Parent ON dbo.Comment(ParentCommentId);

CREATE TABLE dbo.[Like] (
    LikeId INT IDENTITY(1,1) PRIMARY KEY,
    EntityType NVARCHAR(100) NOT NULL,
    EntityId INT NOT NULL,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_Like UNIQUE (EntityType, EntityId, UserId)
);

CREATE INDEX IX_Like_Entity ON dbo.[Like](EntityType, EntityId);
CREATE INDEX IX_Like_User ON dbo.[Like](UserId);

/* ============================================================
   5) QUESTION BANK / EXAM / ATTEMPT
   ============================================================ */
CREATE TABLE dbo.QuestionType (
    QuestionTypeId TINYINT PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL
);

-- Insert common question types
INSERT INTO dbo.QuestionType (QuestionTypeId, Name, Description) VALUES
(1, 'MultipleChoice', 'Trắc nghiệm một đáp án'),
(2, 'MultipleAnswer', 'Trắc nghiệm nhiều đáp án'),
(3, 'TrueFalse', 'Đúng/Sai'),
(4, 'ShortAnswer', 'Câu trả lời ngắn'),
(5, 'Essay', 'Tự luận');

CREATE TABLE dbo.Question (
    QuestionId INT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    CreatedBy INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    TypeId TINYINT NOT NULL FOREIGN KEY REFERENCES dbo.QuestionType(QuestionTypeId),
    Stem NVARCHAR(MAX) NOT NULL,
    Solution NVARCHAR(MAX) NULL,
    Difficulty TINYINT NULL,
    Metadata NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT CK_Question_Difficulty CHECK (Difficulty IS NULL OR Difficulty BETWEEN 1 AND 5)
);

CREATE INDEX IX_Question_Org ON dbo.Question(OrganizationId);
CREATE INDEX IX_Question_Type ON dbo.Question(TypeId);
CREATE INDEX IX_Question_Active ON dbo.Question(IsDeleted) WHERE IsDeleted = 0;

CREATE TABLE dbo.QuestionOption (
    OptionId INT IDENTITY(1,1) PRIMARY KEY,
    QuestionId INT NOT NULL FOREIGN KEY REFERENCES dbo.Question(QuestionId) ON DELETE CASCADE,
    Text NVARCHAR(MAX) NOT NULL,
    IsCorrect BIT NOT NULL DEFAULT 0,
    OrderIndex INT NOT NULL DEFAULT 0,

    CONSTRAINT CK_QuestionOption_Text CHECK (LEN(Text) > 0)
);

CREATE INDEX IX_QuestionOption_Question ON dbo.QuestionOption(QuestionId);

CREATE TABLE dbo.Exam (
    ExamId INT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    CourseId INT NULL FOREIGN KEY REFERENCES dbo.Course(CourseId),
    ClassId INT NULL FOREIGN KEY REFERENCES dbo.Class(ClassId),
    Title NVARCHAR(300) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DurationMinutes INT NULL,

    -- UTC timestamps for exam window
    StartAt DATETIME2(7) NULL,
    EndAt DATETIME2(7) NULL,

    -- Settings
    ShuffleQuestions BIT NOT NULL DEFAULT 0,
    AllowBackNavigation BIT NOT NULL DEFAULT 1,
    ProctoringRequired BIT NOT NULL DEFAULT 0,
    AntiCheatLevel TINYINT NOT NULL DEFAULT 0,

    CreatedBy INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_Exam_Duration CHECK (DurationMinutes IS NULL OR DurationMinutes > 0),
    CONSTRAINT CK_Exam_Time CHECK (EndAt IS NULL OR EndAt > StartAt),
    CONSTRAINT CK_Exam_AntiCheat CHECK (AntiCheatLevel BETWEEN 0 AND 3)
);

CREATE INDEX IX_Exam_Class ON dbo.Exam(ClassId);
CREATE INDEX IX_Exam_Course ON dbo.Exam(CourseId);
CREATE INDEX IX_Exam_Schedule ON dbo.Exam(StartAt, EndAt);

CREATE TABLE dbo.ExamQuestion (
    ExamQuestionId INT IDENTITY(1,1) PRIMARY KEY,
    ExamId INT NOT NULL FOREIGN KEY REFERENCES dbo.Exam(ExamId) ON DELETE CASCADE,
    QuestionId INT NOT NULL FOREIGN KEY REFERENCES dbo.Question(QuestionId),
    Score DECIMAL(6,2) NOT NULL DEFAULT 1.0,
    OrderIndex INT NOT NULL DEFAULT 0,
    RandomizeOptions BIT NOT NULL DEFAULT 0,

    CONSTRAINT CK_ExamQuestion_Score CHECK (Score >= 0),
    CONSTRAINT UQ_ExamQuestion UNIQUE (ExamId, QuestionId)
);

CREATE INDEX IX_ExamQuestion_Exam ON dbo.ExamQuestion(ExamId, OrderIndex);

-- Student Attempt (ALL timestamps in UTC)
CREATE TABLE dbo.StudentExamAttempt (
    AttemptId BIGINT IDENTITY(1,1) PRIMARY KEY,
    ExamId INT NOT NULL FOREIGN KEY REFERENCES dbo.Exam(ExamId),
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    ClassId INT NULL FOREIGN KEY REFERENCES dbo.Class(ClassId),

    -- UTC timestamps
    StartedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    SubmittedAt DATETIME2(7) NULL,

    Status NVARCHAR(50) NOT NULL DEFAULT 'InProgress',
    Score DECIMAL(8,2) NULL,

    -- Tracking info
    IPAddress NVARCHAR(100) NULL,
    BrowserInfo NVARCHAR(MAX) NULL,
    DeviceInfo NVARCHAR(MAX) NULL,
    FocusLostCount INT NOT NULL DEFAULT 0,
    IsCheatingSuspected BIT NOT NULL DEFAULT 0,

    CONSTRAINT CK_Attempt_Status CHECK (Status IN ('InProgress', 'Submitted', 'Graded', 'Cancelled')),
    CONSTRAINT CK_Attempt_Score CHECK (Score IS NULL OR Score >= 0)
);

CREATE INDEX IX_Attempt_Exam ON dbo.StudentExamAttempt(ExamId);
CREATE INDEX IX_Attempt_User ON dbo.StudentExamAttempt(UserId);
CREATE INDEX IX_Attempt_Status ON dbo.StudentExamAttempt(Status);
CREATE INDEX IX_Attempt_Time ON dbo.StudentExamAttempt(StartedAt, SubmittedAt);

CREATE TABLE dbo.StudentAnswer (
    StudentAnswerId BIGINT IDENTITY(1,1) PRIMARY KEY,
    AttemptId BIGINT NOT NULL FOREIGN KEY REFERENCES dbo.StudentExamAttempt(AttemptId) ON DELETE CASCADE,
    QuestionId INT NOT NULL FOREIGN KEY REFERENCES dbo.Question(QuestionId),
    SelectedOptionId INT NULL FOREIGN KEY REFERENCES dbo.QuestionOption(OptionId),
    AnswerText NVARCHAR(MAX) NULL,
    FileId INT NULL FOREIGN KEY REFERENCES dbo.FileStorage(FileId),
    ScoreAwarded DECIMAL(6,2) NULL,
    AutoGraded BIT NOT NULL DEFAULT 0,
    GradedBy INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    GradedAt DATETIME2(7) NULL,
    Feedback NVARCHAR(MAX) NULL,

    CONSTRAINT CK_Answer_Score CHECK (ScoreAwarded IS NULL OR ScoreAwarded >= 0),
    CONSTRAINT UQ_StudentAnswer UNIQUE (AttemptId, QuestionId)
);

CREATE INDEX IX_StudentAnswer_Attempt ON dbo.StudentAnswer(AttemptId);
CREATE INDEX IX_StudentAnswer_Question ON dbo.StudentAnswer(QuestionId);

/* ============================================================
   6) ANTI-CHEAT LOGS (ALL UTC timestamps)
   ============================================================ */
CREATE TABLE dbo.FocusLog (
    FocusLogId BIGINT IDENTITY(1,1) PRIMARY KEY,
    AttemptId BIGINT NOT NULL FOREIGN KEY REFERENCES dbo.StudentExamAttempt(AttemptId) ON DELETE CASCADE,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    OccurredAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    DurationSeconds INT NULL,
    WindowTitle NVARCHAR(1000) NULL,
    Details NVARCHAR(MAX) NULL,

    CONSTRAINT CK_FocusLog_Duration CHECK (DurationSeconds IS NULL OR DurationSeconds >= 0)
);

CREATE INDEX IX_FocusLog_Attempt ON dbo.FocusLog(AttemptId, OccurredAt);
CREATE INDEX IX_FocusLog_User ON dbo.FocusLog(UserId);

CREATE TABLE dbo.WebcamCapture (
    CaptureId BIGINT IDENTITY(1,1) PRIMARY KEY,
    AttemptId BIGINT NOT NULL FOREIGN KEY REFERENCES dbo.StudentExamAttempt(AttemptId) ON DELETE CASCADE,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    FileId INT NULL FOREIGN KEY REFERENCES dbo.FileStorage(FileId),
    CapturedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    FaceDetected BIT NOT NULL DEFAULT 0,
    MatchScore DECIMAL(5,2) NULL,
    Notes NVARCHAR(MAX) NULL,

    CONSTRAINT CK_WebcamCapture_Score CHECK (MatchScore IS NULL OR MatchScore BETWEEN 0 AND 100)
);

CREATE INDEX IX_WebcamCapture_Attempt ON dbo.WebcamCapture(AttemptId, CapturedAt);

CREATE TABLE dbo.CheatingAlert (
    CheatingAlertId BIGINT IDENTITY(1,1) PRIMARY KEY,
    AttemptId BIGINT NULL FOREIGN KEY REFERENCES dbo.StudentExamAttempt(AttemptId),
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    AlertType NVARCHAR(100) NOT NULL,
    Severity TINYINT NOT NULL DEFAULT 1,
    Details NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    HandledBy INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    HandledAt DATETIME2(7) NULL,

    CONSTRAINT CK_CheatingAlert_Severity CHECK (Severity BETWEEN 1 AND 5)
);

CREATE INDEX IX_CheatingAlert_Attempt ON dbo.CheatingAlert(AttemptId);
CREATE INDEX IX_CheatingAlert_User ON dbo.CheatingAlert(UserId);
CREATE INDEX IX_CheatingAlert_Status ON dbo.CheatingAlert(HandledAt);

/* ============================================================
   7) NOTIFICATION (UTC timestamps)
   ============================================================ */
CREATE TABLE dbo.Notification (
    NotificationId BIGINT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    UserId INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    Title NVARCHAR(300) NOT NULL,
    Body NVARCHAR(MAX) NULL,
    Type NVARCHAR(100) NULL,
    Data NVARCHAR(MAX) NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_Notification_Title CHECK (LEN(Title) > 0)
);

CREATE INDEX IX_Notification_User ON dbo.Notification(UserId, IsRead, CreatedAt);

/* ============================================================
   8) PROGRESS & GAMIFICATION
   ============================================================ */
CREATE TABLE dbo.Progress (
    ProgressId BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    ResourceId INT NOT NULL FOREIGN KEY REFERENCES dbo.Resource(ResourceId),
    WatchedPercentage DECIMAL(5,2) NOT NULL DEFAULT 0,
    Completed BIT NOT NULL DEFAULT 0,
    LastSeenAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_Progress_Percentage CHECK (WatchedPercentage BETWEEN 0 AND 100),
    CONSTRAINT UQ_Progress UNIQUE (UserId, ResourceId)
);

CREATE INDEX IX_Progress_User ON dbo.Progress(UserId);
CREATE INDEX IX_Progress_Resource ON dbo.Progress(ResourceId);

CREATE TABLE dbo.GamificationPoint (
    PointId BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    ActivityType NVARCHAR(100) NOT NULL,
    Points INT NOT NULL,
    RelatedEntityType NVARCHAR(100) NULL,
    RelatedEntityId INT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE INDEX IX_GamificationPoint_User ON dbo.GamificationPoint(UserId, CreatedAt);

/* ============================================================
   9) AUDIT LOG (UTC timestamps)
   ============================================================ */
CREATE TABLE dbo.AuditLog (
    AuditId BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL FOREIGN KEY REFERENCES dbo.[User](UserId),
    OrganizationId INT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    Action NVARCHAR(200) NOT NULL,
    EntityType NVARCHAR(200) NULL,
    EntityId NVARCHAR(200) NULL,
    Detail NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE INDEX IX_AuditLog_User ON dbo.AuditLog(UserId, CreatedAt);
CREATE INDEX IX_AuditLog_Entity ON dbo.AuditLog(EntityType, EntityId);
CREATE INDEX IX_AuditLog_Time ON dbo.AuditLog(CreatedAt);

/* ============================================================
   10) BILLING (SUBSCRIPTION, PAYMENT)
   ============================================================ */
CREATE TABLE dbo.Subscription (
    SubscriptionId INT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId INT NOT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    PlanCode NVARCHAR(100) NOT NULL,
    Seats INT NOT NULL DEFAULT 0,

    -- UTC timestamps for subscription period
    StartAt DATETIME2(7) NOT NULL,
    EndAt DATETIME2(7) NULL,

    Status NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_Subscription_Seats CHECK (Seats >= 0),
    CONSTRAINT CK_Subscription_Period CHECK (EndAt IS NULL OR EndAt > StartAt),
    CONSTRAINT CK_Subscription_Status CHECK (Status IN ('Active', 'Expired', 'Cancelled', 'Trial'))
);

CREATE INDEX IX_Subscription_Org ON dbo.Subscription(OrganizationId, Status);
CREATE INDEX IX_Subscription_Period ON dbo.Subscription(StartAt, EndAt);

CREATE TABLE dbo.Payment (
    PaymentId BIGINT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId INT NOT NULL FOREIGN KEY REFERENCES dbo.Organization(OrganizationId),
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(10) NOT NULL DEFAULT 'VND',
    Provider NVARCHAR(100) NULL,
    ProviderTransactionId NVARCHAR(200) NULL,
    Status NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_Payment_Amount CHECK (Amount >= 0),
    CONSTRAINT CK_Payment_Status CHECK (Status IN ('Pending', 'Completed', 'Failed', 'Refunded'))
);

CREATE INDEX IX_Payment_Org ON dbo.Payment(OrganizationId, CreatedAt);
CREATE INDEX IX_Payment_Provider ON dbo.Payment(Provider, ProviderTransactionId);

/* ============================================================
   HELPER FUNCTIONS & VIEWS
   ============================================================ */

-- Function: Convert UTC to Vietnam Time (UTC+7)
GO
CREATE FUNCTION dbo.fn_ConvertToVietnamTime(@utcDateTime DATETIME2(7))
RETURNS DATETIME2(7)
AS
BEGIN
    RETURN DATEADD(HOUR, 7, @utcDateTime);
END;
GO

-- View: User với giờ Việt Nam (chỉ để hiển thị)
GO
CREATE VIEW dbo.vw_User_LocalTime
AS
SELECT
    UserId,
    Email,
    DisplayName,
    IsActive,
    CreatedAt AS CreatedAtUTC,
    dbo.fn_ConvertToVietnamTime(CreatedAt) AS CreatedAtVN,
    LastLoginAt AS LastLoginAtUTC,
    dbo.fn_ConvertToVietnamTime(LastLoginAt) AS LastLoginAtVN
FROM dbo.[User];
GO

-- View: Exam Attempts với giờ Việt Nam
GO
CREATE VIEW dbo.vw_ExamAttempt_LocalTime
AS
SELECT
    AttemptId,
    ExamId,
    UserId,
    Status,
    Score,
    StartedAt AS StartedAtUTC,
    dbo.fn_ConvertToVietnamTime(StartedAt) AS StartedAtVN,
    SubmittedAt AS SubmittedAtUTC,
    dbo.fn_ConvertToVietnamTime(SubmittedAt) AS SubmittedAtVN
FROM dbo.StudentExamAttempt;
GO

/* ============================================================
   STORED PROCEDURES - UTC Best Practices
   ============================================================ */

-- Procedure: Tạo User mới (luôn dùng UTC)
GO
CREATE PROCEDURE dbo.sp_CreateUser
    @Email NVARCHAR(256),
    @DisplayName NVARCHAR(200),
    @OrganizationId INT = NULL,
    @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedEmail NVARCHAR(256) = UPPER(LTRIM(RTRIM(@Email)));

    -- Kiểm tra email tồn tại
    IF EXISTS (SELECT 1 FROM dbo.[User]
               WHERE OrganizationId = @OrganizationId
               AND NormalizedEmail = @NormalizedEmail)
    BEGIN
        THROW 50001, 'Email already exists', 1;
    END

    -- Insert với UTC time
    INSERT INTO dbo.[User] (
        Email, NormalizedEmail, DisplayName, OrganizationId, CreatedBy, CreatedAt
    )
    VALUES (
        @Email, @NormalizedEmail, @DisplayName, @OrganizationId, @CreatedBy, SYSUTCDATETIME()
    );

    SELECT SCOPE_IDENTITY() AS UserId;
END;
GO

-- Procedure: Ghi nhận login (UTC)
GO
CREATE PROCEDURE dbo.sp_RecordLogin
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.[User]
    SET LastLoginAt = SYSUTCDATETIME()
    WHERE UserId = @UserId;

    -- Log audit
    INSERT INTO dbo.AuditLog (UserId, Action, EntityType, EntityId, CreatedAt)
    VALUES (@UserId, 'Login', 'User', CAST(@UserId AS NVARCHAR), SYSUTCDATETIME());
END;
GO

-- Procedure: Submit bài thi
GO
CREATE PROCEDURE dbo.sp_SubmitExamAttempt
    @AttemptId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ExamId INT, @UserId INT;

    SELECT @ExamId = ExamId, @UserId = UserId
    FROM dbo.StudentExamAttempt
    WHERE AttemptId = @AttemptId;

    -- Cập nhật với UTC
    UPDATE dbo.StudentExamAttempt
    SET
        SubmittedAt = SYSUTCDATETIME(),
        Status = 'Submitted'
    WHERE AttemptId = @AttemptId;

    -- Log audit
    INSERT INTO dbo.AuditLog (UserId, Action, EntityType, EntityId, CreatedAt)
    VALUES (@UserId, 'SubmitExam', 'Attempt', CAST(@AttemptId AS NVARCHAR), SYSUTCDATETIME());

    SELECT SubmittedAt FROM dbo.StudentExamAttempt WHERE AttemptId = @AttemptId;
END;
GO

/* ============================================================
   SAMPLE DATA - Demonstration
   ============================================================ */

-- Insert sample organization
INSERT INTO dbo.Organization (Name, Domain, CreatedAt)
VALUES
    (N'Trường Đại học Bách Khoa', 'hcmut.edu.vn', SYSUTCDATETIME()),
    (N'Trường Đại học Khoa học Tự nhiên', 'hcmus.edu.vn', SYSUTCDATETIME());

/* ============================================================
   UTILITY QUERIES - Kiểm tra UTC
   ============================================================ */

-- Kiểm tra thời gian hiện tại
GO
CREATE PROCEDURE dbo.sp_CheckCurrentTimes
AS
BEGIN
    SELECT
        GETDATE() AS [GETDATE (Local Time - SAI)],
        SYSDATETIME() AS [SYSDATETIME (Local Time - SAI)],
        SYSUTCDATETIME() AS [SYSUTCDATETIME (UTC - ĐÚNG)],
        DATEADD(HOUR, 7, SYSUTCDATETIME()) AS [Vietnam Time (UTC+7)];
END;
GO

/* ============================================================
   DOCUMENTATION
   ============================================================ */

-- Thêm extended properties để document
GO
EXEC sys.sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Lưu thời gian tạo theo UTC (Coordinated Universal Time). KHÔNG dùng giờ địa phương.',
    @level0type = N'SCHEMA', @level0name = 'dbo',
    @level1type = N'TABLE',  @level1name = 'User',
    @level2type = N'COLUMN', @level2name = 'CreatedAt';
GO

EXEC sys.sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Lưu lần login cuối theo UTC. Convert sang giờ địa phương khi hiển thị.',
    @level0type = N'SCHEMA', @level0name = 'dbo',
    @level1type = N'TABLE',  @level1name = 'User',
    @level2type = N'COLUMN', @level2name = 'LastLoginAt';
GO

-- BỔ SUNG
-- =============================================
-- 1. REFRESH TOKEN TABLE
-- =============================================
CREATE TABLE dbo.RefreshToken (
    RefreshTokenId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId) ON DELETE CASCADE,
    Token NVARCHAR(500) NOT NULL UNIQUE,
    ExpiresAt DATETIME2(7) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByIp NVARCHAR(50) NULL,
    RevokedAt DATETIME2(7) NULL,
    RevokedByIp NVARCHAR(50) NULL,
    ReplacedByToken NVARCHAR(500) NULL,
    ReasonRevoked NVARCHAR(200) NULL,
    IsExpired AS CASE WHEN ExpiresAt < SYSUTCDATETIME() THEN 1 ELSE 0 END,
    IsRevoked AS CASE WHEN RevokedAt IS NOT NULL THEN 1 ELSE 0 END,
    IsActive AS CASE 
        WHEN RevokedAt IS NULL AND ExpiresAt > SYSUTCDATETIME() THEN 1
        ELSE 0
    END
);

CREATE INDEX IX_RefreshToken_User ON dbo.RefreshToken(UserId);
CREATE INDEX IX_RefreshToken_Token ON dbo.RefreshToken(Token) WHERE RevokedAt IS NULL;
--CREATE INDEX IX_RefreshToken_Active ON dbo.RefreshToken(UserId, IsActive);

-- =============================================
-- 2. TOKEN BLACKLIST TABLE (cho Access Token)
-- =============================================
CREATE TABLE dbo.TokenBlacklist (
    TokenBlacklistId INT IDENTITY(1,1) PRIMARY KEY,
    Token NVARCHAR(MAX) NOT NULL, -- Full JWT token
    UserId INT NOT NULL,
    ExpiresAt DATETIME2(7) NOT NULL,
    RevokedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    Reason NVARCHAR(200) NULL
);

CREATE INDEX IX_TokenBlacklist_User ON dbo.TokenBlacklist(UserId);
CREATE INDEX IX_TokenBlacklist_Expiry ON dbo.TokenBlacklist(ExpiresAt);

--=============================================
-- 3. EMAIL CONFIRMATION TABLE
--=============================================
CREATE TABLE dbo.EmailConfirmationToken (
    TokenId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId) ON DELETE CASCADE,
    Token NVARCHAR(500) NOT NULL UNIQUE,
    ExpiresAt DATETIME2(7) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    UsedAt DATETIME2(7) NULL,
    IsExpired AS CASE WHEN ExpiresAt < SYSUTCDATETIME() THEN 1 ELSE 0 END,
    IsUsed AS CASE WHEN UsedAt IS NOT NULL THEN 1 ELSE 0 END
);

CREATE INDEX IX_EmailConfirmationToken_User ON dbo.EmailConfirmationToken(UserId);
CREATE INDEX IX_EmailConfirmationToken_Token ON dbo.EmailConfirmationToken(Token) WHERE UsedAt IS NULL;
GO
-- =============================================
-- 4. CLEANUP OLD TOKENS (Scheduled job)
-- =============================================
-- Xóa refresh tokens đã hết hạn > 30 ngày
CREATE PROCEDURE dbo.CleanupExpiredTokens
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CutoffDate DATETIME2(7) = DATEADD(DAY, -30, SYSUTCDATETIME());

    -- Delete expired refresh tokens
    DELETE FROM dbo.RefreshToken
    WHERE ExpiresAt < @CutoffDate;

    -- Delete expired blacklist entries
    DELETE FROM dbo.TokenBlacklist
    WHERE ExpiresAt < SYSUTCDATETIME();

    SELECT
        @@ROWCOUNT AS DeletedRows,
        SYSUTCDATETIME() AS CleanupTime;
END;
GO


-- =============================================
-- 5. PASSWORD RESET TOKEN TABLE
-- =============================================
CREATE TABLE dbo.PasswordResetToken (
    TokenId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.[User](UserId) ON DELETE CASCADE,
    Token NVARCHAR(500) NOT NULL UNIQUE,
    ExpiresAt DATETIME2(7) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    UsedAt DATETIME2(7) NULL,
    CreatedByIp NVARCHAR(50) NULL,
    IsExpired AS CASE WHEN ExpiresAt < SYSUTCDATETIME() THEN 1 ELSE 0 END,
    IsUsed AS CASE WHEN UsedAt IS NOT NULL THEN 1 ELSE 0 END
);

CREATE INDEX IX_PasswordResetToken_User ON dbo.PasswordResetToken(UserId);
CREATE INDEX IX_PasswordResetToken_Token ON dbo.PasswordResetToken(Token) WHERE UsedAt IS NULL;
GO