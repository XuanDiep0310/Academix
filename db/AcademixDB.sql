USE [MASTER]
GO

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'AcademixDB')
BEGIN
    CREATE DATABASE AcademixDB;
END
ELSE
BEGIN
   DROP DATABASE AcademixDB;
END
GO

USE AcademixDB
GO

-- =============================================
-- ACADEMIX DATABASE SCHEMA - SQL SERVER
-- =============================================

-- 1. USERS TABLE
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Teacher', 'Student')),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    INDEX IX_Users_Email (Email),
    INDEX IX_Users_Role (Role)
);

-- 2. REFRESH TOKENS TABLE
CREATE TABLE RefreshTokens (
    TokenId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Token NVARCHAR(500) NOT NULL UNIQUE,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    IsRevoked BIT DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    INDEX IX_RefreshTokens_Token (Token),
    INDEX IX_RefreshTokens_UserId (UserId)
);

-- 3. PASSWORD RESET TOKENS TABLE
CREATE TABLE PasswordResetTokens (
    ResetTokenId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Token NVARCHAR(500) NOT NULL UNIQUE,
    ExpiresAt DATETIME2 NOT NULL,
    IsUsed BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    INDEX IX_PasswordResetTokens_Token (Token)
);

-- 4. CLASSES TABLE
CREATE TABLE Classes (
    ClassId INT IDENTITY(1,1) PRIMARY KEY,
    ClassName NVARCHAR(255) NOT NULL,
    ClassCode NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(1000),
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    INDEX IX_Classes_ClassCode (ClassCode)
);

-- 5. CLASS MEMBERS TABLE (Many-to-Many: Users <-> Classes)
CREATE TABLE ClassMembers (
    ClassMemberId INT IDENTITY(1,1) PRIMARY KEY,
    ClassId INT NOT NULL,
    UserId INT NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Teacher', 'Student')),
    JoinedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (ClassId) REFERENCES Classes(ClassId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    UNIQUE (ClassId, UserId),
    INDEX IX_ClassMembers_ClassId (ClassId),
    INDEX IX_ClassMembers_UserId (UserId)
);

-- 6. MATERIALS TABLE
CREATE TABLE Materials (
    MaterialId INT IDENTITY(1,1) PRIMARY KEY,
    ClassId INT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(1000),
    MaterialType NVARCHAR(50) NOT NULL CHECK (MaterialType IN ('PDF', 'Video', 'Image', 'Link', 'Other')),
    FileUrl NVARCHAR(1000),
    FileName NVARCHAR(255),
    FileSize BIGINT,
    UploadedBy INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (ClassId) REFERENCES Classes(ClassId) ON DELETE CASCADE,
    FOREIGN KEY (UploadedBy) REFERENCES Users(UserId),
    INDEX IX_Materials_ClassId (ClassId)
);

-- 7. QUESTION BANK TABLE
CREATE TABLE Questions (
    QuestionId INT IDENTITY(1,1) PRIMARY KEY,
    TeacherId INT NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    QuestionType NVARCHAR(50) DEFAULT 'MultipleChoice',
    DifficultyLevel NVARCHAR(20) CHECK (DifficultyLevel IN ('Easy', 'Medium', 'Hard')),
    Subject NVARCHAR(255),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (TeacherId) REFERENCES Users(UserId),
    INDEX IX_Questions_TeacherId (TeacherId)
);

-- 8. QUESTION OPTIONS TABLE
CREATE TABLE QuestionOptions (
    OptionId INT IDENTITY(1,1) PRIMARY KEY,
    QuestionId INT NOT NULL,
    OptionText NVARCHAR(MAX) NOT NULL,
    IsCorrect BIT NOT NULL DEFAULT 0,
    OptionOrder INT NOT NULL,
    FOREIGN KEY (QuestionId) REFERENCES Questions(QuestionId) ON DELETE CASCADE,
    INDEX IX_QuestionOptions_QuestionId (QuestionId)
);

-- 9. EXAMS TABLE
CREATE TABLE Exams (
    ExamId INT IDENTITY(1,1) PRIMARY KEY,
    ClassId INT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(1000),
    Duration INT NOT NULL, -- minutes
    TotalMarks DECIMAL(5,2) DEFAULT 100,
    StartTime DATETIME2,
    EndTime DATETIME2,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    IsPublished BIT DEFAULT 0,
    FOREIGN KEY (ClassId) REFERENCES Classes(ClassId) ON DELETE CASCADE,
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    INDEX IX_Exams_ClassId (ClassId)
);

-- 10. EXAM QUESTIONS TABLE (Many-to-Many: Exams <-> Questions)
CREATE TABLE ExamQuestions (
    ExamQuestionId INT IDENTITY(1,1) PRIMARY KEY,
    ExamId INT NOT NULL,
    QuestionId INT NOT NULL,
    QuestionOrder INT NOT NULL,
    Marks DECIMAL(5,2) DEFAULT 1,
    FOREIGN KEY (ExamId) REFERENCES Exams(ExamId) ON DELETE CASCADE,
    FOREIGN KEY (QuestionId) REFERENCES Questions(QuestionId),
    UNIQUE (ExamId, QuestionId),
    INDEX IX_ExamQuestions_ExamId (ExamId)
);

-- 11. STUDENT EXAM ATTEMPTS TABLE
CREATE TABLE StudentExamAttempts (
    AttemptId INT IDENTITY(1,1) PRIMARY KEY,
    ExamId INT NOT NULL,
    StudentId INT NOT NULL,
    StartTime DATETIME2 DEFAULT GETDATE(),
    SubmitTime DATETIME2,
    TotalScore DECIMAL(5,2),
    Status NVARCHAR(20) DEFAULT 'InProgress' CHECK (Status IN ('InProgress', 'Submitted', 'Graded')),
    FOREIGN KEY (ExamId) REFERENCES Exams(ExamId) ON DELETE CASCADE,
    FOREIGN KEY (StudentId) REFERENCES Users(UserId),
    INDEX IX_StudentExamAttempts_ExamId (ExamId),
    INDEX IX_StudentExamAttempts_StudentId (StudentId)
);

-- 12. STUDENT ANSWERS TABLE
CREATE TABLE StudentAnswers (
    AnswerId INT IDENTITY(1,1) PRIMARY KEY,
    AttemptId INT NOT NULL,
    QuestionId INT NOT NULL,
    SelectedOptionId INT,
    IsCorrect BIT,
    MarksObtained DECIMAL(5,2),
    AnsweredAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (AttemptId) REFERENCES StudentExamAttempts(AttemptId) ON DELETE CASCADE,
    FOREIGN KEY (QuestionId) REFERENCES Questions(QuestionId),
    FOREIGN KEY (SelectedOptionId) REFERENCES QuestionOptions(OptionId),
    INDEX IX_StudentAnswers_AttemptId (AttemptId)
);
GO

-- =============================================
-- VIEWS FOR COMMON QUERIES
-- =============================================

-- View: Class with member count
CREATE VIEW vw_ClassSummary AS
SELECT
    c.ClassId,
    c.ClassName,
    c.ClassCode,
    c.Description,
    c.CreatedAt,
    COUNT(DISTINCT CASE WHEN cm.Role = 'Teacher' THEN cm.UserId END) AS TeacherCount,
    COUNT(DISTINCT CASE WHEN cm.Role = 'Student' THEN cm.UserId END) AS StudentCount
FROM Classes c
LEFT JOIN ClassMembers cm ON c.ClassId = cm.ClassId
GROUP BY c.ClassId, c.ClassName, c.ClassCode, c.Description, c.CreatedAt;
GO

-- View: Exam Results Summary
CREATE VIEW vw_ExamResults AS
SELECT
    sea.AttemptId,
    sea.ExamId,
    e.Title AS ExamTitle,
    sea.StudentId,
    u.FullName AS StudentName,
    u.Email AS StudentEmail,
    sea.StartTime,
    sea.SubmitTime,
    sea.TotalScore,
    e.TotalMarks,
    sea.Status,
    COUNT(sa.AnswerId) AS TotalAnswered,
    SUM(CASE WHEN sa.IsCorrect = 1 THEN 1 ELSE 0 END) AS CorrectAnswers
FROM StudentExamAttempts sea
INNER JOIN Exams e ON sea.ExamId = e.ExamId
INNER JOIN Users u ON sea.StudentId = u.UserId
LEFT JOIN StudentAnswers sa ON sea.AttemptId = sa.AttemptId
GROUP BY
    sea.AttemptId, sea.ExamId, e.Title, sea.StudentId,
    u.FullName, u.Email, sea.StartTime, sea.SubmitTime,
    sea.TotalScore, e.TotalMarks, sea.Status;
GO
-- =============================================
-- STORED PROCEDURES
-- =============================================

-- SP: Get Student's Classes
CREATE PROCEDURE sp_GetStudentClasses
    @StudentId INT
AS
BEGIN
    SELECT
        c.ClassId,
        c.ClassName,
        c.ClassCode,
        c.Description,
        cm.JoinedAt
    FROM Classes c
    INNER JOIN ClassMembers cm ON c.ClassId = cm.ClassId
    WHERE cm.UserId = @StudentId AND cm.Role = 'Student'
    ORDER BY cm.JoinedAt DESC;
END;
GO

-- SP: Get Teacher's Classes
CREATE PROCEDURE sp_GetTeacherClasses
    @TeacherId INT
AS
BEGIN
    SELECT
        c.ClassId,
        c.ClassName,
        c.ClassCode,
        c.Description,
        cm.JoinedAt,
        (SELECT COUNT(*) FROM ClassMembers WHERE ClassId = c.ClassId AND Role = 'Student') AS StudentCount
    FROM Classes c
    INNER JOIN ClassMembers cm ON c.ClassId = cm.ClassId
    WHERE cm.UserId = @TeacherId AND cm.Role = 'Teacher'
    ORDER BY cm.JoinedAt DESC;
END;
GO

-- SP: Calculate Exam Score
CREATE PROCEDURE sp_CalculateExamScore
    @AttemptId INT
AS
BEGIN
    DECLARE @TotalScore DECIMAL(5,2);

    SELECT @TotalScore = SUM(MarksObtained)
    FROM StudentAnswers
    WHERE AttemptId = @AttemptId;

    UPDATE StudentExamAttempts
    SET TotalScore = @TotalScore,
        Status = 'Graded'
    WHERE AttemptId = @AttemptId;

    SELECT @TotalScore AS TotalScore;
END;
GO

-- =============================================
-- INDEXES FOR PERFORMANCE
-- =============================================

CREATE INDEX IX_Materials_UploadedBy ON Materials(UploadedBy);
CREATE INDEX IX_Exams_CreatedBy ON Exams(CreatedBy);
CREATE INDEX IX_StudentExamAttempts_Status ON StudentExamAttempts(Status);

-- =============================================
-- INITIAL DATA (ADMIN ACCOUNT)
-- =============================================

-- Insert default Admin account
-- Password: 12345678 (hashed with bcrypt)
INSERT INTO Users (Email, PasswordHash, FullName, Role, IsActive)
VALUES ('admin@academix.com', '$2a$12$anEPa0XBY3VHLLwwZh6Jc.TmUgLcxvWbQ07RGNJ5349SpRBMRvYbm', 'System Administrator', 'Admin', 1);

SELECT
    session_id,
    login_name,
    host_name,
    program_name
FROM sys.dm_exec_sessions
WHERE database_id = DB_ID('AcademixDB');