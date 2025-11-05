USE AcademixDB;
GO

-- Seed Permissions
IF NOT EXISTS (SELECT 1 FROM dbo.Permission)
BEGIN
    INSERT INTO dbo.Permission (Name, Code, Description) VALUES
    -- User Management
    ('View Users', 'user.view', 'Can view user list and details'),
    ('Create Users', 'user.create', 'Can create new users'),
    ('Edit Users', 'user.edit', 'Can edit user information'),
    ('Delete Users', 'user.delete', 'Can delete or deactivate users'),
    ('Manage User Roles', 'user.manage_roles', 'Can assign/remove roles from users'),
    
    -- Role Management
    ('View Roles', 'role.view', 'Can view role list and details'),
    ('Create Roles', 'role.create', 'Can create new roles'),
    ('Edit Roles', 'role.edit', 'Can edit role information'),
    ('Delete Roles', 'role.delete', 'Can delete roles'),
    ('Manage Role Permissions', 'role.manage_permissions', 'Can assign/remove permissions from roles'),
    
    -- Organization Management
    ('View Organization', 'organization.view', 'Can view organization details'),
    ('Edit Organization', 'organization.edit', 'Can edit organization information'),
    ('Manage Organization Settings', 'organization.manage_settings', 'Can manage organization settings'),
    
    -- Course Management
    ('View Courses', 'course.view', 'Can view course list and details'),
    ('Create Courses', 'course.create', 'Can create new courses'),
    ('Edit Courses', 'course.edit', 'Can edit course content'),
    ('Delete Courses', 'course.delete', 'Can delete courses'),
    ('Publish Courses', 'course.publish', 'Can publish/unpublish courses');
    
    PRINT 'Permissions seeded successfully';
END
ELSE
BEGIN
    PRINT 'Permissions already exist';
END
GO


-- Seed Default Roles
DECLARE @OrgId INT = NULL; -- NULL = Global roles

-- System Admin Role
IF NOT EXISTS (SELECT 1 FROM dbo.Role WHERE Name = 'System Admin' AND OrganizationId IS NULL)
BEGIN
    INSERT INTO dbo.Role (Name, Description, OrganizationId, CreatedAt)
    VALUES ('System Admin', 'Full system access', NULL, SYSUTCDATETIME());
    
    DECLARE @SystemAdminRoleId INT = SCOPE_IDENTITY();
    
    -- Assign ALL permissions to System Admin
    INSERT INTO dbo.RolePermission (RoleId, PermissionId)
    SELECT @SystemAdminRoleId, PermissionId FROM dbo.Permission;
    
    PRINT 'System Admin role created';
END

-- Organization Admin Role
IF NOT EXISTS (SELECT 1 FROM dbo.Role WHERE Name = 'Organization Admin' AND OrganizationId IS NULL)
BEGIN
    INSERT INTO dbo.Role (Name, Description, OrganizationId, CreatedAt)
    VALUES ('Organization Admin', 'Organization level admin', NULL, SYSUTCDATETIME());
    
    DECLARE @OrgAdminRoleId INT = SCOPE_IDENTITY();
    
    -- Assign permissions
    INSERT INTO dbo.RolePermission (RoleId, PermissionId)
    SELECT @OrgAdminRoleId, PermissionId 
    FROM dbo.Permission 
    WHERE Code IN (
        'user.view', 'user.create', 'user.edit', 'user.manage_roles',
        'role.view', 'role.create', 'role.edit',
        'organization.view', 'organization.edit',
        'course.view', 'course.create', 'course.edit', 'course.delete', 'course.publish'
    );
    
    PRINT 'Organization Admin role created';
END

-- Teacher Role
IF NOT EXISTS (SELECT 1 FROM dbo.Role WHERE Name = 'Teacher' AND OrganizationId IS NULL)
BEGIN
    INSERT INTO dbo.Role (Name, Description, OrganizationId, CreatedAt)
    VALUES ('Teacher', 'Can create and manage courses', NULL, SYSUTCDATETIME());
    
    DECLARE @TeacherRoleId INT = SCOPE_IDENTITY();
    
    INSERT INTO dbo.RolePermission (RoleId, PermissionId)
    SELECT @TeacherRoleId, PermissionId 
    FROM dbo.Permission 
    WHERE Code IN ('user.view', 'course.view', 'course.create', 'course.edit');
    
    PRINT 'Teacher role created';
END

-- Student Role
IF NOT EXISTS (SELECT 1 FROM dbo.Role WHERE Name = 'Student' AND OrganizationId IS NULL)
BEGIN
    INSERT INTO dbo.Role (Name, Description, OrganizationId, CreatedAt)
    VALUES ('Student', 'Can view and enroll in courses', NULL, SYSUTCDATETIME());
    
    DECLARE @StudentRoleId INT = SCOPE_IDENTITY();
    
    INSERT INTO dbo.RolePermission (RoleId, PermissionId)
    SELECT @StudentRoleId, PermissionId 
    FROM dbo.Permission 
    WHERE Code IN ('course.view');
    
    PRINT 'Student role created';
END
GO

-- Seed Default Organization
DECLARE @OrgId INT;

IF NOT EXISTS (SELECT 1 FROM dbo.Organization WHERE Domain = 'default.academix.com')
BEGIN
    INSERT INTO dbo.Organization (Name, Domain, IsActive, CreatedAt)
    VALUES ('Default Organization', 'default.academix.com', 1, SYSUTCDATETIME());
    
    SET @OrgId = SCOPE_IDENTITY();
    PRINT 'Default Organization created';
END
ELSE
BEGIN
    SELECT @OrgId = OrganizationId FROM dbo.Organization WHERE Domain = 'default.academix.com';
END


-- Seed Admin User
DECLARE @AdminEmail NVARCHAR(256) = 'admin@academix.com';
DECLARE @NormalizedEmail NVARCHAR(256) = UPPER(@AdminEmail);

IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE NormalizedEmail = @NormalizedEmail)
BEGIN
    -- NOTE: Password hash/salt cần được generate từ C# code
    -- Tạm thời insert NULL, sau đó update bằng API hoặc C# console app
    INSERT INTO dbo.[User] (
        OrganizationId, Email, NormalizedEmail, UserName, DisplayName,
        PasswordHash, PasswordSalt, IsEmailConfirmed, IsActive, IsDeleted, CreatedAt
    )
    VALUES (
        @OrgId, @AdminEmail, @NormalizedEmail, 'admin', 'System Administrator',
        NULL, NULL, 1, 1, 0, SYSUTCDATETIME()
    );
    
    DECLARE @UserId INT = SCOPE_IDENTITY();
    
    -- Assign System Admin role
    DECLARE @SystemAdminRoleId INT;
    SELECT @SystemAdminRoleId = RoleId FROM dbo.Role WHERE Name = 'System Admin' AND OrganizationId IS NULL;
    
    IF @SystemAdminRoleId IS NOT NULL
    BEGIN
        INSERT INTO dbo.UserRole (UserId, RoleId, AssignedAt)
        VALUES (@UserId, @SystemAdminRoleId, SYSUTCDATETIME());
        
        PRINT 'Admin user created with System Admin role';
        PRINT 'NOTE: Password hash is NULL - need to set via API or console app';
    END
END
ELSE
BEGIN
    PRINT 'Admin user already exists';
END
GO



USE AcademixDB;
GO

-- ============================================================
-- SAMPLE DATASET FOR ACADEMIX DATABASE
-- Tránh xung đột với SeedData.sql
-- ============================================================

SET IDENTITY_INSERT dbo.Organization ON;

-- Thêm Organizations (tránh OrganizationId = 1 đã có)
INSERT INTO dbo.Organization (OrganizationId, Name, Domain, LogoUrl, BillingContact, CreatedAt, IsActive) VALUES
(22, N'Trường Đại học Bách Khoa TP.HCM', 'hcmut.edu.vn', 'https://cdn.academix.com/logos/hcmut.png', 'finance@hcmut.edu.vn', DATEADD(MONTH, -12, SYSUTCDATETIME()), 1),
(33, N'Trường Đại học Khoa học Tự nhiên', 'hcmus.edu.vn', 'https://cdn.academix.com/logos/hcmus.png', 'accounting@hcmus.edu.vn', DATEADD(MONTH, -10, SYSUTCDATETIME()), 1),
(44, N'Trường Đại học Kinh tế TP.HCM', 'ueh.edu.vn', 'https://cdn.academix.com/logos/ueh.png', 'billing@ueh.edu.vn', DATEADD(MONTH, -8, SYSUTCDATETIME()), 1),
(55, N'Học viện Công nghệ Bưu chính Viễn thông', 'ptit.edu.vn', 'https://cdn.academix.com/logos/ptit.png', 'admin@ptit.edu.vn', DATEADD(MONTH, -6, SYSUTCDATETIME()), 1);

SET IDENTITY_INSERT dbo.Organization OFF;
GO

-- ============================================================
-- USERS (Mật khẩu cần hash từ code, tạm để NULL)
-- ============================================================
SET IDENTITY_INSERT dbo.[User] ON;

INSERT INTO dbo.[User] (UserId, OrganizationId, Email, NormalizedEmail, UserName, DisplayName, Phone, Bio, IsEmailConfirmed, IsActive, IsDeleted, CreatedAt) VALUES
-- HCMUT Teachers
(2, 22, 'nguyen.van.a@hcmut.edu.vn', 'NGUYEN.VAN.A@HCMUT.EDU.VN', 'nguyenvana', N'TS. Nguyễn Văn A', '0901234567', N'Giảng viên khoa Khoa học Máy tính', 1, 1, 0, DATEADD(MONTH, -11, SYSUTCDATETIME())),
(3, 22, 'tran.thi.b@hcmut.edu.vn', 'TRAN.THI.B@HCMUT.EDU.VN', 'tranthib', N'PGS.TS. Trần Thị B', '0912345678', N'Giảng viên cao cấp, chuyên ngành AI', 1, 1, 0, DATEADD(MONTH, -10, SYSUTCDATETIME())),
(4, 22, 'le.van.c@hcmut.edu.vn', 'LE.VAN.C@HCMUT.EDU.VN', 'levanc', N'ThS. Lê Văn C', '0923456789', N'Giảng viên trẻ, nhiệt huyết', 1, 1, 0, DATEADD(MONTH, -9, SYSUTCDATETIME())),

-- HCMUT Students
(5, 22, 'student1@hcmut.edu.vn', 'STUDENT1@HCMUT.EDU.VN', 'student1', N'Phạm Minh Đức', '0934567890', N'Sinh viên năm 3 Khoa học Máy tính', 1, 1, 0, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(6, 22, 'student2@hcmut.edu.vn', 'STUDENT2@HCMUT.EDU.VN', 'student2', N'Hoàng Thị Mai', '0945678901', N'Sinh viên năm 2', 1, 1, 0, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(7, 22, 'student3@hcmut.edu.vn', 'STUDENT3@HCMUT.EDU.VN', 'student3', N'Trần Quốc Anh', '0956789012', N'Sinh viên năm 4', 1, 1, 0, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(8, 22, 'student4@hcmut.edu.vn', 'STUDENT4@HCMUT.EDU.VN', 'student4', N'Lê Thị Hương', '0967890123', N'Sinh viên năm 3', 1, 1, 0, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(9, 22, 'student5@hcmut.edu.vn', 'STUDENT5@HCMUT.EDU.VN', 'student5', N'Nguyễn Văn Khoa', '0978901234', NULL, 1, 1, 0, DATEADD(MONTH, -6, SYSUTCDATETIME())),
(10, 22, 'student6@hcmut.edu.vn', 'STUDENT6@HCMUT.EDU.VN', 'student6', N'Võ Thị Lan', '0989012345', NULL, 1, 1, 0, DATEADD(MONTH, -6, SYSUTCDATETIME())),

-- HCMUS Users
(11, 33, 'pham.van.d@hcmus.edu.vn', 'PHAM.VAN.D@HCMUS.EDU.VN', 'phamvand', N'PGS.TS. Phạm Văn D', '0990123456', N'Chuyên gia Toán học ứng dụng', 1, 1, 0, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(12, 33, 'student7@hcmus.edu.vn', 'STUDENT7@HCMUS.EDU.VN', 'student7', N'Đặng Minh Tuấn', '0901234568', NULL, 1, 1, 0, DATEADD(MONTH, -5, SYSUTCDATETIME())),
(13, 33, 'student8@hcmus.edu.vn', 'STUDENT8@HCMUS.EDU.VN', 'student8', N'Bùi Thị Nga', '0912345679', NULL, 1, 1, 0, DATEADD(MONTH, -5, SYSUTCDATETIME())),

-- UEH Users
(14, 44, 'nguyen.thi.e@ueh.edu.vn', 'NGUYEN.THI.E@UEH.EDU.VN', 'nguyenthie', N'TS. Nguyễn Thị E', '0923456780', N'Giảng viên Kinh tế học', 1, 1, 0, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(15, 44, 'student9@ueh.edu.vn', 'STUDENT9@UEH.EDU.VN', 'student9', N'Trương Văn Nam', '0934567891', NULL, 1, 1, 0, DATEADD(MONTH, -4, SYSUTCDATETIME())),

-- PTIT Users
(16, 55, 'le.thi.f@ptit.edu.vn', 'LE.THI.F@PTIT.EDU.VN', 'lethif', N'ThS. Lê Thị F', '0945678902', N'Giảng viên Mạng máy tính', 1, 1, 0, DATEADD(MONTH, -6, SYSUTCDATETIME())),
(17, 55, 'student10@ptit.edu.vn', 'STUDENT10@PTIT.EDU.VN', 'student10', N'Phan Thị Thu', '0956789013', NULL, 1, 1, 0, DATEADD(MONTH, -3, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.[User] OFF;
GO

-- ============================================================
-- USER ROLES (Dùng RoleId từ SeedData.sql)
-- ============================================================
-- System Admin role = 1, Org Admin = 2, Teacher = 3, Student = 4

INSERT INTO dbo.UserRole (UserId, RoleId, AssignedAt, AssignedBy) VALUES
-- Teachers (RoleId = 3)
(2, 3, DATEADD(MONTH, -11, SYSUTCDATETIME()), 1),
(3, 3, DATEADD(MONTH, -10, SYSUTCDATETIME()), 1),
(4, 3, DATEADD(MONTH, -9, SYSUTCDATETIME()), 1),
(11, 3, DATEADD(MONTH, -9, SYSUTCDATETIME()), 1),
(14, 3, DATEADD(MONTH, -7, SYSUTCDATETIME()), 1),
(16, 3, DATEADD(MONTH, -6, SYSUTCDATETIME()), 1),

-- Students (RoleId = 4)
(5, 4, DATEADD(MONTH, -8, SYSUTCDATETIME()), 1),
(6, 4, DATEADD(MONTH, -8, SYSUTCDATETIME()), 1),
(7, 4, DATEADD(MONTH, -7, SYSUTCDATETIME()), 1),
(8, 4, DATEADD(MONTH, -7, SYSUTCDATETIME()), 1),
(9, 4, DATEADD(MONTH, -6, SYSUTCDATETIME()), 1),
(10, 4, DATEADD(MONTH, -6, SYSUTCDATETIME()), 1),
(12, 4, DATEADD(MONTH, -5, SYSUTCDATETIME()), 1),
(13, 4, DATEADD(MONTH, -5, SYSUTCDATETIME()), 1),
(15, 4, DATEADD(MONTH, -4, SYSUTCDATETIME()), 1),
(17, 4, DATEADD(MONTH, -3, SYSUTCDATETIME()), 1);
GO

-- ============================================================
-- COURSES
-- ============================================================
SET IDENTITY_INSERT dbo.Course ON;

INSERT INTO dbo.Course (CourseId, OrganizationId, Code, Title, ShortDescription, LongDescription, CreatedBy, IsPublished, CreatedAt) VALUES
(1, 22, 'CS101', N'Nhập môn Lập trình', N'Khóa học lập trình cơ bản cho người mới bắt đầu', N'Khóa học cung cấp kiến thức nền tảng về lập trình, cấu trúc dữ liệu và giải thuật cơ bản. Sinh viên sẽ học cách tư duy logic và giải quyết vấn đề bằng code.', 2, 1, DATEADD(MONTH, -10, SYSUTCDATETIME())),
(2, 22, 'CS201', N'Cấu trúc Dữ liệu và Giải thuật', N'Khóa học nâng cao về thuật toán', N'Học về các cấu trúc dữ liệu quan trọng: mảng, danh sách liên kết, cây, đồ thị và các thuật toán tìm kiếm, sắp xếp.', 2, 1, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(3, 22, 'CS301', N'Trí tuệ Nhân tạo', N'Giới thiệu về AI và Machine Learning', N'Khóa học về các khái niệm cơ bản trong AI, machine learning, neural networks và ứng dụng thực tế.', 3, 1, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(4, 22, 'CS202', N'Lập trình Hướng đối tượng', N'OOP với Java và C#', N'Học các nguyên lý OOP: encapsulation, inheritance, polymorphism, abstraction và design patterns.', 4, 1, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(5, 22, 'CS302', N'Phát triển Web Full-stack', N'Xây dựng ứng dụng web hiện đại', N'Học HTML, CSS, JavaScript, React, Node.js, Express và MongoDB để xây dựng ứng dụng web hoàn chỉnh.', 2, 1, DATEADD(MONTH, -6, SYSUTCDATETIME())),
(6, 33, 'MATH101', N'Giải tích 1', N'Toán cao cấp cơ bản', N'Giới hạn, đạo hàm, tích phân và ứng dụng trong thực tế.', 11, 1, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(7, 33, 'MATH201', N'Đại số Tuyến tính', N'Ma trận và Vector', N'Học về không gian vector, ma trận, định thức, giá trị riêng và ứng dụng.', 11, 1, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(8, 44, 'ECON101', N'Kinh tế Vi mô', N'Nguyên lý kinh tế cơ bản', N'Cung cầu, thị trường, hành vi người tiêu dùng và doanh nghiệp.', 14, 1, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(9, 44, 'ECON201', N'Kinh tế Vĩ mô', N'Kinh tế quốc gia và quốc tế', N'GDP, lạm phát, thất nghiệp, chính sách tài khóa và tiền tệ.', 14, 1, DATEADD(MONTH, -6, SYSUTCDATETIME())),
(10, 55, 'NET101', N'Mạng Máy tính Cơ bản', N'Nguyên lý truyền thông mạng', N'Mô hình OSI, TCP/IP, routing, switching và bảo mật mạng cơ bản.', 16, 1, DATEADD(MONTH, -6, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.Course OFF;
GO

-- ============================================================
-- CLASSES
-- ============================================================
SET IDENTITY_INSERT dbo.Class ON;

INSERT INTO dbo.Class (ClassId, CourseId, OrganizationId, Title, TeacherId, StartDate, EndDate, EnrollmentCode, MaxStudents, IsActive, CreatedAt) VALUES
(1, 1, 22, N'CS101 - Học kỳ 1/2024', 2, '2024-09-01', '2024-12-15', 'CS101HK1', 50, 1, DATEADD(MONTH, -10, SYSUTCDATETIME())),
(2, 1, 22, N'CS101 - Học kỳ 2/2024', 2, '2025-01-10', '2025-05-20', 'CS101HK2', 50, 1, DATEADD(MONTH, -3, SYSUTCDATETIME())),
(3, 2, 22, N'CS201 - Học kỳ 1/2024', 2, '2024-09-01', '2024-12-15', 'CS201HK1', 40, 1, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(4, 3, 22, N'CS301 - Lớp AI1', 3, '2024-10-01', '2025-01-31', 'AI1-2024', 30, 1, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(5, 4, 22, N'CS202 - OOP Java', 4, '2024-09-15', '2024-12-30', 'OOP-JAVA', 45, 1, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(6, 5, 22, N'CS302 - Web Dev 2024', 2, '2024-10-01', '2025-02-15', 'WEB2024', 35, 1, DATEADD(MONTH, -6, SYSUTCDATETIME())),
(7, 6, 33, N'MATH101 - Giải tích 1A', 11, '2024-09-01', '2024-12-20', 'MATH101A', 60, 1, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(8, 7, 33, N'MATH201 - Đại số TT', 11, '2024-10-01', '2025-01-30', 'MATH201', 40, 1, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(9, 8, 44, N'ECON101 - Lớp A', 14, '2024-09-15', '2024-12-25', 'ECON101A', 50, 1, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(10, 10, 55, N'NET101 - Mạng cơ bản', 16, '2024-10-01', '2025-01-31', 'NET101', 35, 1, DATEADD(MONTH, -6, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.Class OFF;
GO

-- ============================================================
-- ENROLLMENTS
-- ============================================================
SET IDENTITY_INSERT dbo.Enrollment ON;

INSERT INTO dbo.Enrollment (EnrollmentId, ClassId, UserId, RoleInClass, JoinedAt, IsApproved, IsActive) VALUES
-- Class 1 (CS101 HK1)
(1, 1, 5, 'Student', DATEADD(MONTH, -10, SYSUTCDATETIME()), 1, 1),
(2, 1, 6, 'Student', DATEADD(MONTH, -10, SYSUTCDATETIME()), 1, 1),
(3, 1, 7, 'Student', DATEADD(MONTH, -9, SYSUTCDATETIME()), 1, 1),
(4, 1, 8, 'Student', DATEADD(MONTH, -9, SYSUTCDATETIME()), 1, 1),

-- Class 2 (CS101 HK2)
(5, 2, 9, 'Student', DATEADD(MONTH, -3, SYSUTCDATETIME()), 1, 1),
(6, 2, 10, 'Student', DATEADD(MONTH, -3, SYSUTCDATETIME()), 1, 1),

-- Class 3 (CS201)
(7, 3, 5, 'Student', DATEADD(MONTH, -9, SYSUTCDATETIME()), 1, 1),
(8, 3, 7, 'Student', DATEADD(MONTH, -9, SYSUTCDATETIME()), 1, 1),
(9, 3, 8, 'Student', DATEADD(MONTH, -9, SYSUTCDATETIME()), 1, 1),

-- Class 4 (AI)
(10, 4, 5, 'Student', DATEADD(MONTH, -8, SYSUTCDATETIME()), 1, 1),
(11, 4, 7, 'Student', DATEADD(MONTH, -8, SYSUTCDATETIME()), 1, 1),

-- Class 5 (OOP)
(12, 5, 6, 'Student', DATEADD(MONTH, -7, SYSUTCDATETIME()), 1, 1),
(13, 5, 8, 'Student', DATEADD(MONTH, -7, SYSUTCDATETIME()), 1, 1),
(14, 5, 9, 'Student', DATEADD(MONTH, -7, SYSUTCDATETIME()), 1, 1),

-- Class 6 (Web Dev)
(15, 6, 5, 'Student', DATEADD(MONTH, -6, SYSUTCDATETIME()), 1, 1),
(16, 6, 6, 'Student', DATEADD(MONTH, -6, SYSUTCDATETIME()), 1, 1),
(17, 6, 10, 'Student', DATEADD(MONTH, -6, SYSUTCDATETIME()), 1, 1),

-- Class 7 (MATH101)
(18, 7, 12, 'Student', DATEADD(MONTH, -9, SYSUTCDATETIME()), 1, 1),
(19, 7, 13, 'Student', DATEADD(MONTH, -9, SYSUTCDATETIME()), 1, 1),

-- Class 9 (ECON101)
(20, 9, 15, 'Student', DATEADD(MONTH, -7, SYSUTCDATETIME()), 1, 1);

SET IDENTITY_INSERT dbo.Enrollment OFF;
GO

-- ============================================================
-- FILE STORAGE
-- ============================================================
SET IDENTITY_INSERT dbo.FileStorage ON;

INSERT INTO dbo.FileStorage (FileId, OrganizationId, UploadedBy, FileName, BlobUri, StorageProvider, ContentType, ContentLength, Checksum, CreatedAt) VALUES
(1, 22, 2, 'lecture1_intro.pdf', 'https://storage.academix.com/files/cs101/lecture1.pdf', 'Azure', 'application/pdf', 2457600, 'abc123def456', DATEADD(MONTH, -10, SYSUTCDATETIME())),
(2, 22, 2, 'lecture2_variables.pdf', 'https://storage.academix.com/files/cs101/lecture2.pdf', 'Azure', 'application/pdf', 1843200, 'def456ghi789', DATEADD(MONTH, -9, SYSUTCDATETIME())),
(3, 22, 3, 'ai_basics.pptx', 'https://storage.academix.com/files/cs301/ai_basics.pptx', 'Azure', 'application/vnd.openxmlformats-officedocument.presentationml.presentation', 5242880, 'ghi789jkl012', DATEADD(MONTH, -8, SYSUTCDATETIME())),
(4, 22, 2, 'dsa_sorting.pdf', 'https://storage.academix.com/files/cs201/sorting.pdf', 'Azure', 'application/pdf', 3145728, 'jkl012mno345', DATEADD(MONTH, -9, SYSUTCDATETIME())),
(5, 22, 4, 'oop_inheritance.pdf', 'https://storage.academix.com/files/cs202/inheritance.pdf', 'Azure', 'application/pdf', 2097152, 'mno345pqr678', DATEADD(MONTH, -7, SYSUTCDATETIME())),
(6, 33, 11, 'calculus_limits.pdf', 'https://storage.academix.com/files/math101/limits.pdf', 'Azure', 'application/pdf', 1572864, 'pqr678stu901', DATEADD(MONTH, -9, SYSUTCDATETIME())),
(7, 44, 14, 'micro_supply_demand.pdf', 'https://storage.academix.com/files/econ101/supply_demand.pdf', 'Azure', 'application/pdf', 2621440, 'stu901vwx234', DATEADD(MONTH, -7, SYSUTCDATETIME())),
(8, 22, 2, 'video_intro_programming.mp4', 'https://storage.academix.com/videos/cs101/intro.mp4', 'Azure', 'video/mp4', 52428800, 'vwx234yz567', DATEADD(MONTH, -10, SYSUTCDATETIME())),
(9, 22, 3, 'video_ml_basics.mp4', 'https://storage.academix.com/videos/cs301/ml_basics.mp4', 'Azure', 'video/mp4', 104857600, 'yz567abc890', DATEADD(MONTH, -8, SYSUTCDATETIME())),
(10, 55, 16, 'networking_osi_model.pdf', 'https://storage.academix.com/files/net101/osi.pdf', 'Azure', 'application/pdf', 1835008, 'abc890def123', DATEADD(MONTH, -6, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.FileStorage OFF;
GO

-- ============================================================
-- RESOURCES
-- ============================================================
SET IDENTITY_INSERT dbo.Resource ON;

INSERT INTO dbo.Resource (ResourceId, OrganizationId, CourseId, ClassId, Title, Description, ResourceType, Visibility, CreatedBy, CreatedAt) VALUES
(1, 22, 1, 1, N'Bài giảng 1: Giới thiệu Lập trình', N'Slide giới thiệu về lập trình và tư duy thuật toán', 'Document', 'ClassOnly', 2, DATEADD(MONTH, -10, SYSUTCDATETIME())),
(2, 22, 1, 1, N'Video: Lập trình là gì?', N'Video giới thiệu khái niệm lập trình', 'Video', 'ClassOnly', 2, DATEADD(MONTH, -10, SYSUTCDATETIME())),
(3, 22, 1, 1, N'Bài giảng 2: Biến và Kiểu dữ liệu', N'Tài liệu về variables và data types', 'Document', 'ClassOnly', 2, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(4, 22, 2, 3, N'Thuật toán Sắp xếp', N'Các thuật toán sắp xếp cơ bản', 'Document', 'ClassOnly', 2, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(5, 22, 3, 4, N'AI Fundamentals', N'Giới thiệu AI và ML', 'Document', 'Public', 3, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(6, 22, 3, 4, N'Video: Machine Learning cơ bản', N'Video hướng dẫn ML', 'Video', 'Public', 3, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(7, 22, 4, 5, N'OOP: Kế thừa trong Java', N'Bài giảng về Inheritance', 'Document', 'ClassOnly', 4, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(8, 33, 6, 7, N'Giới hạn và Liên tục', N'Lý thuyết về limits', 'Document', 'ClassOnly', 11, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(9, 44, 8, 9, N'Cung và Cầu', N'Mô hình cung cầu cơ bản', 'Document', 'ClassOnly', 14, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(10, 55, 10, 10, N'Mô hình OSI', N'7 tầng của OSI model', 'Document', 'ClassOnly', 16, DATEADD(MONTH, -6, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.Resource OFF;
GO

-- ============================================================
-- RESOURCE VERSIONS
-- ============================================================
SET IDENTITY_INSERT dbo.ResourceVersion ON;

INSERT INTO dbo.ResourceVersion (ResourceVersionId, ResourceId, FileId, VersionNumber, Notes, CreatedBy, CreatedAt) VALUES
(1, 1, 1, 1, N'Phiên bản đầu tiên', 2, DATEADD(MONTH, -10, SYSUTCDATETIME())),
(2, 2, 8, 1, N'Video intro programming', 2, DATEADD(MONTH, -10, SYSUTCDATETIME())),
(3, 3, 2, 1, N'Bài giảng Variables', 2, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(4, 4, 4, 1, N'Sorting algorithms v1', 2, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(5, 5, 3, 1, N'AI Basics presentation', 3, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(6, 6, 9, 1, N'ML video tutorial', 3, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(7, 7, 5, 1, N'Java Inheritance', 4, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(8, 8, 6, 1, N'Calculus limits theory', 11, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(9, 9, 7, 1, N'Supply and demand model', 14, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(10, 10, 10, 1, N'OSI model explanation', 16, DATEADD(MONTH, -6, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.ResourceVersion OFF;
GO

-- Update CurrentVersionId in Resource table
UPDATE dbo.Resource SET CurrentVersionId = 1 WHERE ResourceId = 1;
UPDATE dbo.Resource SET CurrentVersionId = 2 WHERE ResourceId = 2;
UPDATE dbo.Resource SET CurrentVersionId = 3 WHERE ResourceId = 3;
UPDATE dbo.Resource SET CurrentVersionId = 4 WHERE ResourceId = 4;
UPDATE dbo.Resource SET CurrentVersionId = 5 WHERE ResourceId = 5;
UPDATE dbo.Resource SET CurrentVersionId = 6 WHERE ResourceId = 6;
UPDATE dbo.Resource SET CurrentVersionId = 7 WHERE ResourceId = 7;
UPDATE dbo.Resource SET CurrentVersionId = 8 WHERE ResourceId = 8;
UPDATE dbo.Resource SET CurrentVersionId = 9 WHERE ResourceId = 9;
UPDATE dbo.Resource SET CurrentVersionId = 10 WHERE ResourceId = 10;
GO

-- ============================================================
-- QUESTIONS
-- ============================================================
SET IDENTITY_INSERT dbo.Question ON;

INSERT INTO dbo.Question (QuestionId, OrganizationId, CreatedBy, TypeId, Stem, Solution, Difficulty, CreatedAt) VALUES
-- CS101 Questions
(1, 22, 2, 1, N'Trong lập trình, biến (variable) là gì?', N'Biến là vùng nhớ được đặt tên để lưu trữ dữ liệu', 1, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(2, 22, 2, 1, N'Kiểu dữ liệu nào dùng để lưu số nguyên?', N'int (integer)', 1, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(3, 22, 2, 2, N'Chọn các ngôn ngữ lập trình hướng đối tượng:', N'Java, C++, Python, C#', 2, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(4, 22, 2, 3, N'Python là ngôn ngữ compiled (biên dịch)?', N'Sai - Python là ngôn ngữ interpreted', 1, DATEADD(MONTH, -9, SYSUTCDATETIME())),
(5, 22, 2, 1, N'Thuật toán là gì?', N'Tập hợp các bước rõ ràng để giải quyết một vấn đề', 1, DATEADD(MONTH, -9, SYSUTCDATETIME())),

-- CS201 Questions (DSA)
(6, 22, 2, 1, N'Độ phức tạp thời gian của Quick Sort trung bình là?', N'O(n log n)', 3, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(7, 22, 2, 1, N'Cấu trúc dữ liệu nào hoạt động theo nguyên tắc FIFO?', N'Queue (Hàng đợi)', 2, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(8, 22, 2, 1, N'Cấu trúc dữ liệu nào hoạt động theo nguyên tắc LIFO?', N'Stack (Ngăn xếp)', 2, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(9, 22, 2, 3, N'Binary Search chỉ hoạt động trên mảng đã sắp xếp?', N'Đúng', 2, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(10, 22, 2, 2, N'Chọn các thuật toán sắp xếp có độ phức tạp O(n²):', N'Bubble Sort, Selection Sort, Insertion Sort', 3, DATEADD(MONTH, -8, SYSUTCDATETIME())),

-- CS301 Questions (AI)
(11, 22, 3, 1, N'Machine Learning là gì?', N'Khả năng máy tính học từ dữ liệu mà không cần lập trình tường minh', 2, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(12, 22, 3, 2, N'Chọn các loại Machine Learning:', N'Supervised Learning, Unsupervised Learning, Reinforcement Learning', 2, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(13, 22, 3, 1, N'Neural Network lấy cảm hứng từ đâu?', N'Mạng lưới thần kinh trong não người', 2, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(14, 22, 3, 3, N'Deep Learning là tập con của Machine Learning?', N'Đúng', 2, DATEADD(MONTH, -7, SYSUTCDATETIME())),
(15, 22, 3, 4, N'Giải thích khái niệm Overfitting trong ML', N'Overfitting xảy ra khi model học quá kỹ training data, kể cả noise, dẫn đến performance kém trên data mới', 3, DATEADD(MONTH, -7, SYSUTCDATETIME())),

-- MATH Questions
(16, 33, 11, 1, N'Đạo hàm của hàm số f(x) = x² là?', N'f''(x) = 2x', 2, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(17, 33, 11, 1, N'Giới hạn của (sin x)/x khi x tiến về 0 là?', N'1', 3, DATEADD(MONTH, -8, SYSUTCDATETIME())),
(18, 33, 11, 3, N'Tích phân là phép toán ngược của đạo hàm?', N'Đúng', 2, DATEADD(MONTH, -8, SYSUTCDATETIME())),

-- ECON Questions
(19, 44, 14, 1, N'Định luật cầu nói rằng:', N'Khi giá tăng, lượng cầu giảm (với các yếu tố khác không đổi)', 2, DATEADD(MONTH, -6, SYSUTCDATETIME())),
(20, 44, 14, 1, N'GDP viết tắt của từ gì?', N'Gross Domestic Product (Tổng sản phẩm quốc nội)', 1, DATEADD(MONTH, -6, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.Question OFF;
GO

-- ============================================================
-- QUESTION OPTIONS
-- ============================================================
SET IDENTITY_INSERT dbo.QuestionOption ON;

INSERT INTO dbo.QuestionOption (OptionId, QuestionId, Text, IsCorrect, OrderIndex) VALUES
-- Question 1
(1, 1, N'Vùng nhớ được đặt tên để lưu trữ dữ liệu', 1, 1),
(2, 1, N'Một hàm toán học', 0, 2),
(3, 1, N'Một loại thuật toán', 0, 3),
(4, 1, N'Một kiểu dữ liệu', 0, 4),

-- Question 2
(5, 2, N'string', 0, 1),
(6, 2, N'int', 1, 2),
(7, 2, N'float', 0, 3),
(8, 2, N'boolean', 0, 4),

-- Question 3 (Multiple Answer)
(9, 3, N'Java', 1, 1),
(10, 3, N'C', 0, 2),
(11, 3, N'C++', 1, 3),
(12, 3, N'Python', 1, 4),
(13, 3, N'Assembly', 0, 5),
(14, 3, N'C#', 1, 6),

-- Question 4 (True/False)
(15, 4, N'Đúng', 0, 1),
(16, 4, N'Sai', 1, 2),

-- Question 5
(17, 5, N'Một ngôn ngữ lập trình', 0, 1),
(18, 5, N'Tập hợp các bước để giải quyết vấn đề', 1, 2),
(19, 5, N'Một kiểu dữ liệu', 0, 3),
(20, 5, N'Một công cụ lập trình', 0, 4),

-- Question 6
(21, 6, N'O(n)', 0, 1),
(22, 6, N'O(n log n)', 1, 2),
(23, 6, N'O(n²)', 0, 3),
(24, 6, N'O(log n)', 0, 4),

-- Question 7
(25, 7, N'Stack', 0, 1),
(26, 7, N'Queue', 1, 2),
(27, 7, N'Tree', 0, 3),
(28, 7, N'Graph', 0, 4),

-- Question 8
(29, 8, N'Queue', 0, 1),
(30, 8, N'Stack', 1, 2),
(31, 8, N'Array', 0, 3),
(32, 8, N'Linked List', 0, 4),

-- Question 9 (True/False)
(33, 9, N'Đúng', 1, 1),
(34, 9, N'Sai', 0, 2),

-- Question 10 (Multiple Answer)
(35, 10, N'Bubble Sort', 1, 1),
(36, 10, N'Quick Sort', 0, 2),
(37, 10, N'Selection Sort', 1, 3),
(38, 10, N'Merge Sort', 0, 4),
(39, 10, N'Insertion Sort', 1, 5),

-- Question 11
(40, 11, N'Lập trình máy tính', 0, 1),
(41, 11, N'Khả năng máy tính học từ dữ liệu', 1, 2),
(42, 11, N'Trí tuệ nhân tạo', 0, 3),
(43, 11, N'Thuật toán tìm kiếm', 0, 4),

-- Question 12 (Multiple Answer)
(44, 12, N'Supervised Learning', 1, 1),
(45, 12, N'Unsupervised Learning', 1, 2),
(46, 12, N'Reinforcement Learning', 1, 3),
(47, 12, N'Random Learning', 0, 4),

-- Question 13
(48, 13, N'Máy tính', 0, 1),
(49, 13, N'Mạng lưới thần kinh trong não', 1, 2),
(50, 13, N'Internet', 0, 3),
(51, 13, N'Mạng xã hội', 0, 4),

-- Question 14 (True/False)
(52, 14, N'Đúng', 1, 1),
(53, 14, N'Sai', 0, 2),

-- Question 16
(54, 16, N'2x', 1, 1),
(55, 16, N'x²', 0, 2),
(56, 16, N'x', 0, 3),
(57, 16, N'2', 0, 4),

-- Question 17
(58, 17, N'0', 0, 1),
(59, 17, N'1', 1, 2),
(60, 17, N'∞', 0, 3),
(61, 17, N'Không tồn tại', 0, 4),

-- Question 18 (True/False)
(62, 18, N'Đúng', 1, 1),
(63, 18, N'Sai', 0, 2),

-- Question 19
(64, 19, N'Khi giá tăng, lượng cầu tăng', 0, 1),
(65, 19, N'Khi giá tăng, lượng cầu giảm', 1, 2),
(66, 19, N'Giá không ảnh hưởng đến cầu', 0, 3),
(67, 19, N'Cầu luôn không đổi', 0, 4),

-- Question 20
(68, 20, N'Government Development Plan', 0, 1),
(69, 20, N'Gross Domestic Product', 1, 2),
(70, 20, N'General Data Protection', 0, 3),
(71, 20, N'Global Development Program', 0, 4);

SET IDENTITY_INSERT dbo.QuestionOption OFF;
GO

-- ============================================================
-- EXAMS
-- ============================================================
SET IDENTITY_INSERT dbo.Exam ON;

INSERT INTO dbo.Exam (ExamId, OrganizationId, CourseId, ClassId, Title, Description, DurationMinutes, StartAt, EndAt, ShuffleQuestions, AllowBackNavigation, CreatedBy, CreatedAt) VALUES
(1, 22, 1, 1, N'Kiểm tra Giữa kỳ CS101', N'Kiểm tra kiến thức cơ bản về lập trình', 60, DATEADD(DAY, -30, SYSUTCDATETIME()), DATEADD(DAY, -29, SYSUTCDATETIME()), 1, 1, 2, DATEADD(DAY, -35, SYSUTCDATETIME())),
(2, 22, 1, 1, N'Thi Cuối kỳ CS101', N'Thi cuối kỳ môn Nhập môn Lập trình', 90, DATEADD(DAY, -10, SYSUTCDATETIME()), DATEADD(DAY, -9, SYSUTCDATETIME()), 1, 0, 2, DATEADD(DAY, -15, SYSUTCDATETIME())),
(3, 22, 2, 3, N'Quiz 1: Độ phức tạp thuật toán', N'Kiểm tra nhanh về Big O notation', 30, DATEADD(DAY, -25, SYSUTCDATETIME()), DATEADD(DAY, -24, SYSUTCDATETIME()), 1, 1, 2, DATEADD(DAY, -28, SYSUTCDATETIME())),
(4, 22, 2, 3, N'Thi Giữa kỳ CS201', N'Kiểm tra về cấu trúc dữ liệu', 75, DATEADD(DAY, -20, SYSUTCDATETIME()), DATEADD(DAY, -19, SYSUTCDATETIME()), 1, 1, 2, DATEADD(DAY, -25, SYSUTCDATETIME())),
(5, 22, 3, 4, N'Quiz: Machine Learning Basics', N'Kiểm tra kiến thức ML cơ bản', 45, DATEADD(DAY, -15, SYSUTCDATETIME()), DATEADD(DAY, -14, SYSUTCDATETIME()), 1, 1, 3, DATEADD(DAY, -20, SYSUTCDATETIME())),
(6, 22, 3, 4, N'Thi Giữa kỳ AI', N'Thi giữa kỳ môn Trí tuệ Nhân tạo', 90, DATEADD(DAY, -5, SYSUTCDATETIME()), DATEADD(DAY, -4, SYSUTCDATETIME()), 1, 0, 3, DATEADD(DAY, -10, SYSUTCDATETIME())),
(7, 33, 6, 7, N'Kiểm tra Giới hạn', N'Bài kiểm tra về giới hạn và liên tục', 60, DATEADD(DAY, -22, SYSUTCDATETIME()), DATEADD(DAY, -21, SYSUTCDATETIME()), 0, 1, 11, DATEADD(DAY, -25, SYSUTCDATETIME())),
(8, 44, 8, 9, N'Quiz Kinh tế Vi mô', N'Kiểm tra nhanh về cung cầu', 30, DATEADD(DAY, -18, SYSUTCDATETIME()), DATEADD(DAY, -17, SYSUTCDATETIME()), 1, 1, 14, DATEADD(DAY, -20, SYSUTCDATETIME())),
(9, 22, 1, 2, N'Quiz 1 CS101 HK2', N'Kiểm tra đầu kỳ', 30, DATEADD(DAY, 5, SYSUTCDATETIME()), DATEADD(DAY, 6, SYSUTCDATETIME()), 1, 1, 2, DATEADD(DAY, -2, SYSUTCDATETIME())),
(10, 22, 4, 5, N'Thi OOP Giữa kỳ', N'Kiểm tra về OOP concepts', 75, DATEADD(DAY, -12, SYSUTCDATETIME()), DATEADD(DAY, -11, SYSUTCDATETIME()), 1, 1, 4, DATEADD(DAY, -15, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.Exam OFF;
GO

-- ============================================================
-- EXAM QUESTIONS
-- ============================================================
SET IDENTITY_INSERT dbo.ExamQuestion ON;

INSERT INTO dbo.ExamQuestion (ExamQuestionId, ExamId, QuestionId, Score, OrderIndex, RandomizeOptions) VALUES
-- Exam 1: CS101 Midterm (Questions 1-5)
(1, 1, 1, 2.0, 1, 1),
(2, 1, 2, 2.0, 2, 1),
(3, 1, 3, 3.0, 3, 1),
(4, 1, 4, 1.0, 4, 0),
(5, 1, 5, 2.0, 5, 1),

-- Exam 2: CS101 Final (Questions 1-5)
(6, 2, 1, 1.5, 1, 1),
(7, 2, 2, 1.5, 2, 1),
(8, 2, 3, 2.0, 3, 1),
(9, 2, 4, 1.0, 4, 0),
(10, 2, 5, 2.0, 5, 1),

-- Exam 3: CS201 Quiz (Questions 6-8)
(11, 3, 6, 3.0, 1, 1),
(12, 3, 7, 2.0, 2, 1),
(13, 3, 8, 2.0, 3, 1),

-- Exam 4: CS201 Midterm (Questions 6-10)
(14, 4, 6, 2.0, 1, 1),
(15, 4, 7, 2.0, 2, 1),
(16, 4, 8, 2.0, 3, 1),
(17, 4, 9, 1.5, 4, 0),
(18, 4, 10, 2.5, 5, 1),

-- Exam 5: AI Quiz (Questions 11-13)
(19, 5, 11, 3.0, 1, 1),
(20, 5, 12, 3.5, 2, 1),
(21, 5, 13, 2.5, 3, 1),

-- Exam 6: AI Midterm (Questions 11-15)
(22, 6, 11, 2.0, 1, 1),
(23, 6, 12, 2.0, 2, 1),
(24, 6, 13, 2.0, 3, 1),
(25, 6, 14, 1.5, 4, 0),
(26, 6, 15, 2.5, 5, 0),

-- Exam 7: Math Quiz (Questions 16-18)
(27, 7, 16, 3.0, 1, 1),
(28, 7, 17, 4.0, 2, 1),
(29, 7, 18, 1.5, 3, 0),

-- Exam 8: Econ Quiz (Questions 19-20)
(30, 8, 19, 5.0, 1, 1),
(31, 8, 20, 3.0, 2, 1),

-- Exam 9: CS101 HK2 Quiz (Questions 1-3)
(32, 9, 1, 3.0, 1, 1),
(33, 9, 2, 3.0, 2, 1),
(34, 9, 3, 4.0, 3, 1),

-- Exam 10: OOP Midterm (Questions 1-5)
(35, 10, 1, 1.5, 1, 1),
(36, 10, 2, 1.5, 2, 1),
(37, 10, 3, 2.5, 3, 1),
(38, 10, 4, 1.5, 4, 0),
(39, 10, 5, 2.0, 5, 1);

SET IDENTITY_INSERT dbo.ExamQuestion OFF;
GO

-- ============================================================
-- STUDENT EXAM ATTEMPTS
-- ============================================================
SET IDENTITY_INSERT dbo.StudentExamAttempt ON;

INSERT INTO dbo.StudentExamAttempt (AttemptId, ExamId, UserId, ClassId, StartedAt, SubmittedAt, Status, Score, IPAddress, FocusLostCount) VALUES
-- Exam 1 attempts (CS101 Midterm)
(1, 1, 5, 1, DATEADD(HOUR, -720, SYSUTCDATETIME()), DATEADD(HOUR, -719, SYSUTCDATETIME()), 'Graded', 8.5, '192.168.1.10', 2),
(2, 1, 6, 1, DATEADD(HOUR, -720, SYSUTCDATETIME()), DATEADD(HOUR, -719, SYSUTCDATETIME()), 'Graded', 7.0, '192.168.1.11', 0),
(3, 1, 7, 1, DATEADD(HOUR, -720, SYSUTCDATETIME()), DATEADD(HOUR, -719, SYSUTCDATETIME()), 'Graded', 9.0, '192.168.1.12', 1),
(4, 1, 8, 1, DATEADD(HOUR, -720, SYSUTCDATETIME()), DATEADD(HOUR, -719, SYSUTCDATETIME()), 'Graded', 6.5, '192.168.1.13', 3),

-- Exam 2 attempts (CS101 Final)
(5, 2, 5, 1, DATEADD(HOUR, -240, SYSUTCDATETIME()), DATEADD(HOUR, -238, SYSUTCDATETIME()), 'Graded', 7.5, '192.168.1.10', 1),
(6, 2, 6, 1, DATEADD(HOUR, -240, SYSUTCDATETIME()), DATEADD(HOUR, -238, SYSUTCDATETIME()), 'Graded', 8.0, '192.168.1.11', 0),
(7, 2, 7, 1, DATEADD(HOUR, -240, SYSUTCDATETIME()), DATEADD(HOUR, -238, SYSUTCDATETIME()), 'Graded', 9.5, '192.168.1.12', 0),
(8, 2, 8, 1, DATEADD(HOUR, -240, SYSUTCDATETIME()), DATEADD(HOUR, -238, SYSUTCDATETIME()), 'Graded', 7.0, '192.168.1.13', 2),

-- Exam 3 attempts (CS201 Quiz)
(9, 3, 5, 3, DATEADD(HOUR, -600, SYSUTCDATETIME()), DATEADD(HOUR, -600, SYSUTCDATETIME()), 'Graded', 6.5, '192.168.1.10', 0),
(10, 3, 7, 3, DATEADD(HOUR, -600, SYSUTCDATETIME()), DATEADD(HOUR, -600, SYSUTCDATETIME()), 'Graded', 7.0, '192.168.1.12', 1),
(11, 3, 8, 3, DATEADD(HOUR, -600, SYSUTCDATETIME()), DATEADD(HOUR, -600, SYSUTCDATETIME()), 'Graded', 5.5, '192.168.1.13', 0),

-- Exam 4 attempts (CS201 Midterm)
(12, 4, 5, 3, DATEADD(HOUR, -480, SYSUTCDATETIME()), DATEADD(HOUR, -479, SYSUTCDATETIME()), 'Graded', 8.0, '192.168.1.10', 1),
(13, 4, 7, 3, DATEADD(HOUR, -480, SYSUTCDATETIME()), DATEADD(HOUR, -479, SYSUTCDATETIME()), 'Graded', 9.0, '192.168.1.12', 0),
(14, 4, 8, 3, DATEADD(HOUR, -480, SYSUTCDATETIME()), DATEADD(HOUR, -479, SYSUTCDATETIME()), 'Graded', 7.5, '192.168.1.13', 2),

-- Exam 5 attempts (AI Quiz)
(15, 5, 5, 4, DATEADD(HOUR, -360, SYSUTCDATETIME()), DATEADD(HOUR, -360, SYSUTCDATETIME()), 'Graded', 8.5, '192.168.1.10', 0),
(16, 5, 7, 4, DATEADD(HOUR, -360, SYSUTCDATETIME()), DATEADD(HOUR, -360, SYSUTCDATETIME()), 'Graded', 9.0, '192.168.1.12', 0),

-- Exam 6 attempts (AI Midterm)
(17, 6, 5, 4, DATEADD(HOUR, -120, SYSUTCDATETIME()), DATEADD(HOUR, -118, SYSUTCDATETIME()), 'Graded', 8.0, '192.168.1.10', 1),
(18, 6, 7, 4, DATEADD(HOUR, -120, SYSUTCDATETIME()), DATEADD(HOUR, -118, SYSUTCDATETIME()), 'Graded', 9.5, '192.168.1.12', 0),

-- Exam 7 attempts (Math Quiz)
(19, 7, 12, 7, DATEADD(HOUR, -528, SYSUTCDATETIME()), DATEADD(HOUR, -527, SYSUTCDATETIME()), 'Graded', 7.5, '192.168.2.20', 1),
(20, 7, 13, 7, DATEADD(HOUR, -528, SYSUTCDATETIME()), DATEADD(HOUR, -527, SYSUTCDATETIME()), 'Graded', 8.5, '192.168.2.21', 0);

SET IDENTITY_INSERT dbo.StudentExamAttempt OFF;
GO

-- ============================================================
-- STUDENT ANSWERS
-- ============================================================
SET IDENTITY_INSERT dbo.StudentAnswer ON;

INSERT INTO dbo.StudentAnswer (StudentAnswerId, AttemptId, QuestionId, SelectedOptionId, ScoreAwarded, AutoGraded, GradedAt) VALUES
-- Attempt 1 (Student 5, Exam 1) - Score: 8.5/10
(1, 1, 1, 1, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(2, 1, 2, 6, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(3, 1, 3, 9, 2.5, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())), -- Partial credit (selected 2/4 correct)
(4, 1, 4, 16, 1.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(5, 1, 5, 18, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),

-- Attempt 2 (Student 6, Exam 1) - Score: 7.0/10
(6, 2, 1, 1, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(7, 2, 2, 6, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(8, 2, 3, 9, 1.5, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())), -- Partial
(9, 2, 4, 15, 0.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())), -- Wrong
(10, 2, 5, 18, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),

-- Attempt 3 (Student 7, Exam 1) - Score: 9.0/10
(11, 3, 1, 1, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(12, 3, 2, 6, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(13, 3, 3, 9, 3.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())), -- Full credit
(14, 3, 4, 16, 1.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(15, 3, 5, 17, 0.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())), -- Wrong

-- Attempt 4 (Student 8, Exam 1) - Score: 6.5/10
(16, 4, 1, 2, 0.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())), -- Wrong
(17, 4, 2, 6, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(18, 4, 3, 9, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(19, 4, 4, 16, 1.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),
(20, 4, 5, 18, 2.0, 1, DATEADD(HOUR, -719, SYSUTCDATETIME())),

-- Attempt 5 (Student 5, Exam 2 - Final) - Score: 7.5/10
(21, 5, 1, 1, 1.5, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(22, 5, 2, 6, 1.5, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(23, 5, 3, 9, 2.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(24, 5, 4, 15, 0.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(25, 5, 5, 18, 2.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),

-- Attempt 6 (Student 6, Exam 2) - Score: 8.0/10
(26, 6, 1, 1, 1.5, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(27, 6, 2, 6, 1.5, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(28, 6, 3, 9, 2.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(29, 6, 4, 16, 1.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(30, 6, 5, 18, 2.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),

-- Attempt 7 (Student 7, Exam 2) - Score: 9.5/10
(31, 7, 1, 1, 1.5, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(32, 7, 2, 6, 1.5, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(33, 7, 3, 9, 2.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(34, 7, 4, 16, 1.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(35, 7, 5, 18, 2.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),

-- Attempt 8 (Student 8, Exam 2) - Score: 7.0/10
(36, 8, 1, 1, 1.5, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(37, 8, 2, 5, 0.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())), -- Wrong
(38, 8, 3, 9, 2.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(39, 8, 4, 16, 1.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),
(40, 8, 5, 18, 2.0, 1, DATEADD(HOUR, -238, SYSUTCDATETIME())),

-- CS201 Quiz attempts
(41, 9, 6, 22, 3.0, 1, DATEADD(HOUR, -600, SYSUTCDATETIME())),
(42, 9, 7, 26, 2.0, 1, DATEADD(HOUR, -600, SYSUTCDATETIME())),
(43, 9, 8, 30, 2.0, 1, DATEADD(HOUR, -600, SYSUTCDATETIME())),

(44, 10, 6, 22, 3.0, 1, DATEADD(HOUR, -600, SYSUTCDATETIME())),
(45, 10, 7, 26, 2.0, 1, DATEADD(HOUR, -600, SYSUTCDATETIME())),
(46, 10, 8, 30, 2.0, 1, DATEADD(HOUR, -600, SYSUTCDATETIME())),

(47, 11, 6, 21, 0.0, 1, DATEADD(HOUR, -600, SYSUTCDATETIME())), -- Wrong
(48, 11, 7, 26, 2.0, 1, DATEADD(HOUR, -600, SYSUTCDATETIME())),
(49, 11, 8, 30, 2.0, 1, DATEADD(HOUR, -600, SYSUTCDATETIME())),

-- CS201 Midterm attempts
(50, 12, 6, 22, 2.0, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(51, 12, 7, 26, 2.0, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(52, 12, 8, 30, 2.0, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(53, 12, 9, 33, 1.5, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(54, 12, 10, 35, 1.5, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),

(55, 13, 6, 22, 2.0, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(56, 13, 7, 26, 2.0, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(57, 13, 8, 30, 2.0, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(58, 13, 9, 33, 1.5, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(59, 13, 10, 35, 2.5, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),

(60, 14, 6, 22, 2.0, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(61, 14, 7, 26, 2.0, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(62, 14, 8, 29, 0.0, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())), -- Wrong
(63, 14, 9, 33, 1.5, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),
(64, 14, 10, 35, 2.0, 1, DATEADD(HOUR, -479, SYSUTCDATETIME())),

-- AI Quiz attempts
(65, 15, 11, 41, 3.0, 1, DATEADD(HOUR, -360, SYSUTCDATETIME())),
(66, 15, 12, 44, 3.5, 1, DATEADD(HOUR, -360, SYSUTCDATETIME())),
(67, 15, 13, 49, 2.5, 1, DATEADD(HOUR, -360, SYSUTCDATETIME())),

(68, 16, 11, 41, 3.0, 1, DATEADD(HOUR, -360, SYSUTCDATETIME())),
(69, 16, 12, 44, 3.5, 1, DATEADD(HOUR, -360, SYSUTCDATETIME())),
(70, 16, 13, 49, 2.5, 1, DATEADD(HOUR, -360, SYSUTCDATETIME())),

-- AI Midterm attempts
(71, 17, 11, 41, 2.0, 1, DATEADD(HOUR, -118, SYSUTCDATETIME())),
(72, 17, 12, 44, 2.0, 1, DATEADD(HOUR, -118, SYSUTCDATETIME())),
(73, 17, 13, 49, 2.0, 1, DATEADD(HOUR, -118, SYSUTCDATETIME())),
(74, 17, 14, 52, 1.5, 1, DATEADD(HOUR, -118, SYSUTCDATETIME())),
(75, 17, 15, NULL, 1.5, 0, NULL), -- Essay - needs manual grading

(76, 18, 11, 41, 2.0, 1, DATEADD(HOUR, -118, SYSUTCDATETIME())),
(77, 18, 12, 44, 2.0, 1, DATEADD(HOUR, -118, SYSUTCDATETIME())),
(78, 18, 13, 49, 2.0, 1, DATEADD(HOUR, -118, SYSUTCDATETIME())),
(79, 18, 14, 52, 1.5, 1, DATEADD(HOUR, -118, SYSUTCDATETIME())),
(80, 18, 15, NULL, 2.5, 0, NULL), -- Essay - manually graded

-- Math Quiz attempts
(81, 19, 16, 54, 3.0, 1, DATEADD(HOUR, -527, SYSUTCDATETIME())),
(82, 19, 17, 59, 4.0, 1, DATEADD(HOUR, -527, SYSUTCDATETIME())),
(83, 19, 18, 62, 1.5, 1, DATEADD(HOUR, -527, SYSUTCDATETIME())),

(84, 20, 16, 54, 3.0, 1, DATEADD(HOUR, -527, SYSUTCDATETIME())),
(85, 20, 17, 59, 4.0, 1, DATEADD(HOUR, -527, SYSUTCDATETIME())),
(86, 20, 18, 62, 1.5, 1, DATEADD(HOUR, -527, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.StudentAnswer OFF;
GO

-- ============================================================
-- COMMENTS (on various entities)
-- ============================================================
SET IDENTITY_INSERT dbo.Comment ON;

INSERT INTO dbo.Comment (CommentId, EntityType, EntityId, ParentCommentId, UserId, Content, CreatedAt) VALUES
-- Comments on Resources
(1, 'Resource', 1, NULL, 5, N'Bài giảng rất dễ hiểu, cảm ơn thầy!', DATEADD(DAY, -25, SYSUTCDATETIME())),
(2, 'Resource', 1, 1, 2, N'Cảm ơn em, chúc em học tốt!', DATEADD(DAY, -25, SYSUTCDATETIME())),
(3, 'Resource', 2, NULL, 6, N'Video này hơi dài, có thể tóm tắt được không ạ?', DATEADD(DAY, -24, SYSUTCDATETIME())),
(4, 'Resource', 3, NULL, 7, N'Phần biến và kiểu dữ liệu giải thích rất chi tiết', DATEADD(DAY, -23, SYSUTCDATETIME())),
(5, 'Resource', 4, NULL, 8, N'Thuật toán sắp xếp còn phức tạp quá', DATEADD(DAY, -22, SYSUTCDATETIME())),
(6, 'Resource', 5, NULL, 5, N'AI thật thú vị, em muốn tìm hiểu thêm về Deep Learning', DATEADD(DAY, -20, SYSUTCDATETIME())),
(7, 'Resource', 5, 6, 3, N'Em có thể tham khảo khóa học nâng cao của khoa nhé', DATEADD(DAY, -20, SYSUTCDATETIME())),
(8, 'Resource', 6, NULL, 7, N'Video ML rất hay, giải thích kỹ từng concept', DATEADD(DAY, -19, SYSUTCDATETIME())),
(9, 'Resource', 7, NULL, 9, N'Inheritance trong Java khá dễ hiểu', DATEADD(DAY, -18, SYSUTCDATETIME())),
(10, 'Resource', 8, NULL, 12, N'Giới hạn là phần khó nhất của Giải tích', DATEADD(DAY, -21, SYSUTCDATETIME())),

-- Comments on Exams
(11, 'Exam', 1, NULL, 5, N'Đề thi vừa sức, thời gian hợp lý', DATEADD(DAY, -29, SYSUTCDATETIME())),
(12, 'Exam', 1, NULL, 6, N'Câu 3 hơi khó', DATEADD(DAY, -29, SYSUTCDATETIME())),
(13, 'Exam', 2, NULL, 7, N'Đề cuối kỳ khó hơn giữa kỳ nhiều', DATEADD(DAY, -9, SYSUTCDATETIME())),
(14, 'Exam', 3, NULL, 5, N'Quiz nhanh gọn, test được kiến thức cơ bản', DATEADD(DAY, -24, SYSUTCDATETIME())),
(15, 'Exam', 4, NULL, 8, N'Cấu trúc dữ liệu cần ôn kỹ lý thuyết', DATEADD(DAY, -19, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.Comment OFF;
GO

-- ============================================================
-- LIKES (on various entities)
-- ============================================================
SET IDENTITY_INSERT dbo.[Like] ON;

INSERT INTO dbo.[Like] (LikeId, EntityType, EntityId, UserId, CreatedAt) VALUES
-- Likes on Resources
(1, 'Resource', 1, 5, DATEADD(DAY, -25, SYSUTCDATETIME())),
(2, 'Resource', 1, 6, DATEADD(DAY, -25, SYSUTCDATETIME())),
(3, 'Resource', 1, 7, DATEADD(DAY, -24, SYSUTCDATETIME())),
(4, 'Resource', 2, 5, DATEADD(DAY, -24, SYSUTCDATETIME())),
(5, 'Resource', 2, 8, DATEADD(DAY, -23, SYSUTCDATETIME())),
(6, 'Resource', 3, 6, DATEADD(DAY, -23, SYSUTCDATETIME())),
(7, 'Resource', 4, 7, DATEADD(DAY, -22, SYSUTCDATETIME())),
(8, 'Resource', 5, 5, DATEADD(DAY, -20, SYSUTCDATETIME())),
(9, 'Resource', 5, 7, DATEADD(DAY, -20, SYSUTCDATETIME())),
(10, 'Resource', 6, 5, DATEADD(DAY, -19, SYSUTCDATETIME())),
(11, 'Resource', 6, 7, DATEADD(DAY, -19, SYSUTCDATETIME())),
(12, 'Resource', 7, 9, DATEADD(DAY, -18, SYSUTCDATETIME())),
(13, 'Resource', 8, 12, DATEADD(DAY, -21, SYSUTCDATETIME())),

-- Likes on Comments
(14, 'Comment', 1, 6, DATEADD(DAY, -25, SYSUTCDATETIME())),
(15, 'Comment', 1, 7, DATEADD(DAY, -25, SYSUTCDATETIME())),
(16, 'Comment', 6, 7, DATEADD(DAY, -20, SYSUTCDATETIME())),
(17, 'Comment', 8, 5, DATEADD(DAY, -19, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.[Like] OFF;
GO

-- ============================================================
-- PROGRESS (Video watching progress)
-- ============================================================
SET IDENTITY_INSERT dbo.Progress ON;

INSERT INTO dbo.Progress (ProgressId, UserId, ResourceId, WatchedPercentage, Completed, LastSeenAt) VALUES
(1, 5, 2, 100.00, 1, DATEADD(DAY, -24, SYSUTCDATETIME())),
(2, 5, 6, 85.50, 0, DATEADD(DAY, -19, SYSUTCDATETIME())),
(3, 6, 2, 45.00, 0, DATEADD(DAY, -23, SYSUTCDATETIME())),
(4, 7, 2, 100.00, 1, DATEADD(DAY, -24, SYSUTCDATETIME())),
(5, 7, 6, 100.00, 1, DATEADD(DAY, -19, SYSUTCDATETIME())),
(6, 8, 2, 60.00, 0, DATEADD(DAY, -22, SYSUTCDATETIME())),
(7, 5, 1, 100.00, 1, DATEADD(DAY, -25, SYSUTCDATETIME())),
(8, 6, 1, 100.00, 1, DATEADD(DAY, -25, SYSUTCDATETIME())),
(9, 7, 4, 100.00, 1, DATEADD(DAY, -22, SYSUTCDATETIME())),
(10, 8, 4, 70.00, 0, DATEADD(DAY, -21, SYSUTCDATETIME())),
(11, 12, 8, 100.00, 1, DATEADD(DAY, -21, SYSUTCDATETIME())),
(12, 13, 8, 55.00, 0, DATEADD(DAY, -20, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.Progress OFF;
GO

-- ============================================================
-- GAMIFICATION POINTS
-- ============================================================
SET IDENTITY_INSERT dbo.GamificationPoint ON;

INSERT INTO dbo.GamificationPoint (PointId, UserId, OrganizationId, ActivityType, Points, RelatedEntityType, RelatedEntityId, CreatedAt) VALUES
-- Student 5 points
(1, 5, 22, 'CompleteLesson', 10, 'Resource', 1, DATEADD(DAY, -25, SYSUTCDATETIME())),
(2, 5, 22, 'CompleteVideo', 15, 'Resource', 2, DATEADD(DAY, -24, SYSUTCDATETIME())),
(3, 5, 22, 'CompleteExam', 50, 'Exam', 1, DATEADD(DAY, -30, SYSUTCDATETIME())),
(4, 5, 22, 'HighScore', 20, 'Exam', 1, DATEADD(DAY, -30, SYSUTCDATETIME())),
(5, 5, 22, 'CompleteExam', 50, 'Exam', 2, DATEADD(DAY, -10, SYSUTCDATETIME())),
(6, 5, 22, 'Comment', 5, 'Resource', 1, DATEADD(DAY, -25, SYSUTCDATETIME())),

-- Student 6 points
(7, 6, 22, 'CompleteLesson', 10, 'Resource', 1, DATEADD(DAY, -25, SYSUTCDATETIME())),
(8, 6, 22, 'CompleteExam', 50, 'Exam', 1, DATEADD(DAY, -30, SYSUTCDATETIME())),
(9, 6, 22, 'CompleteExam', 50, 'Exam', 2, DATEADD(DAY, -10, SYSUTCDATETIME())),
(10, 6, 22, 'Comment', 5, 'Resource', 2, DATEADD(DAY, -24, SYSUTCDATETIME())),

-- Student 7 points
(11, 7, 22, 'CompleteVideo', 15, 'Resource', 2, DATEADD(DAY, -24, SYSUTCDATETIME())),
(12, 7, 22, 'CompleteExam', 50, 'Exam', 1, DATEADD(DAY, -30, SYSUTCDATETIME())),
(13, 7, 22, 'HighScore', 20, 'Exam', 1, DATEADD(DAY, -30, SYSUTCDATETIME())),
(14, 7, 22, 'CompleteExam', 50, 'Exam', 2, DATEADD(DAY, -10, SYSUTCDATETIME())),
(15, 7, 22, 'PerfectScore', 30, 'Exam', 2, DATEADD(DAY, -10, SYSUTCDATETIME())),
(16, 7, 22, 'CompleteVideo', 15, 'Resource', 6, DATEADD(DAY, -19, SYSUTCDATETIME())),

-- Student 8 points
(17, 8, 22, 'CompleteExam', 50, 'Exam', 1, DATEADD(DAY, -30, SYSUTCDATETIME())),
(18, 8, 22, 'CompleteExam', 50, 'Exam', 2, DATEADD(DAY, -10, SYSUTCDATETIME())),

-- Student 12 points (HCMUS)
(19, 12, 33, 'CompleteLesson', 10, 'Resource', 8, DATEADD(DAY, -21, SYSUTCDATETIME())),
(20, 12, 33, 'CompleteExam', 50, 'Exam', 7, DATEADD(DAY, -22, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.GamificationPoint OFF;
GO

-- ============================================================
-- NOTIFICATIONS
-- ============================================================
SET IDENTITY_INSERT dbo.Notification ON;

INSERT INTO dbo.Notification (NotificationId, OrganizationId, UserId, Title, Body, Type, IsRead, CreatedAt) VALUES
(1, 22, 5, N'Bài thi mới', N'Giáo viên vừa tạo bài thi "Kiểm tra Giữa kỳ CS101"', 'ExamCreated', 1, DATEADD(DAY, -35, SYSUTCDATETIME())),
(2, 22, 5, N'Điểm đã được công bố', N'Điểm thi Giữa kỳ CS101 của bạn đã được công bố: 8.5/10', 'GradePublished', 1, DATEADD(DAY, -29, SYSUTCDATETIME())),
(3, 22, 5, N'Tài liệu mới', N'Giáo viên đã upload tài liệu "Bài giảng 2: Biến và Kiểu dữ liệu"', 'ResourceAdded', 1, DATEADD(DAY, -9, SYSUTCDATETIME())),
(4, 22, 5, N'Bài thi sắp diễn ra', N'Bài thi "Thi Cuối kỳ CS101" sẽ bắt đầu trong 24 giờ', 'ExamReminder', 1, DATEADD(DAY, -11, SYSUTCDATETIME())),
(5, 22, 5, N'Điểm cuối kỳ', N'Điểm thi Cuối kỳ CS101: 7.5/10', 'GradePublished', 1, DATEADD(DAY, -9, SYSUTCDATETIME())),
(6, 22, 6, N'Bài thi mới', N'Giáo viên vừa tạo bài thi "Kiểm tra Giữa kỳ CS101"', 'ExamCreated', 1, DATEADD(DAY, -35, SYSUTCDATETIME())),
(7, 22, 6, N'Điểm đã được công bố', N'Điểm thi Giữa kỳ CS101 của bạn: 7.0/10', 'GradePublished', 1, DATEADD(DAY, -29, SYSUTCDATETIME())),
(8, 22, 7, N'Chào mừng tham gia lớp', N'Bạn đã được thêm vào lớp "CS101 - Học kỳ 1/2024"', 'EnrollmentApproved', 1, DATEADD(DAY, -45, SYSUTCDATETIME())),
(9, 22, 7, N'Điểm xuất sắc!', N'Chúc mừng! Bạn đạt 9.5/10 trong kỳ thi Cuối kỳ CS101', 'GradePublished', 1, DATEADD(DAY, -9, SYSUTCDATETIME())),
(10, 22, 8, N'Comment mới', N'Có người đã trả lời comment của bạn', 'CommentReply', 0, DATEADD(DAY, -5, SYSUTCDATETIME())),
(11, 33, 12, N'Tài liệu mới', N'Tài liệu "Giới hạn và Liên tục" đã được thêm', 'ResourceAdded', 1, DATEADD(DAY, -22, SYSUTCDATETIME())),
(12, 33, 12, N'Bài thi sắp tới', N'Bài kiểm tra Giới hạn sẽ diễn ra vào tuần sau', 'ExamReminder', 0, DATEADD(DAY, -2, SYSUTCDATETIME())),
(13, 44, 15, N'Chào mừng đến Academix', N'Bạn đã đăng ký thành công tài khoản', 'Welcome', 1, DATEADD(MONTH, -4, SYSUTCDATETIME())),
(14, 22, 9, N'Quiz mới', N'Quiz 1 CS101 HK2 đã được tạo', 'ExamCreated', 0, DATEADD(DAY, -2, SYSUTCDATETIME())),
(15, 22, 10, N'Nhắc nhở làm bài', N'Bạn chưa hoàn thành bài tập tuần này', 'Reminder', 0, DATEADD(DAY, -1, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.Notification OFF;
GO

-- ============================================================
-- AUDIT LOG
-- ============================================================
SET IDENTITY_INSERT dbo.AuditLog ON;

INSERT INTO dbo.AuditLog (AuditId, UserId, OrganizationId, Action, EntityType, EntityId, Detail, CreatedAt) VALUES
(1, 1, NULL, 'CreateOrganization', 'Organization', '2', N'Created HCMUT organization', DATEADD(MONTH, -12, SYSUTCDATETIME())),
(2, 1, NULL, 'CreateOrganization', 'Organization', '3', N'Created HCMUS organization', DATEADD(MONTH, -10, SYSUTCDATETIME())),
(3, 2, 22, 'CreateCourse', 'Course', '1', N'Created CS101 course', DATEADD(MONTH, -10, SYSUTCDATETIME())),
(4, 2, 22, 'CreateClass', 'Class', '1', N'Created CS101 HK1 class', DATEADD(MONTH, -10, SYSUTCDATETIME())),
(5, 5, 22, 'EnrollClass', 'Enrollment', '1', N'Student enrolled in CS101 HK1', DATEADD(MONTH, -10, SYSUTCDATETIME())),
(6, 2, 22, 'UploadResource', 'Resource', '1', N'Uploaded lecture1_intro.pdf', DATEADD(MONTH, -10, SYSUTCDATETIME())),
(7, 2, 22, 'CreateExam', 'Exam', '1', N'Created Midterm exam for CS101', DATEADD(DAY, -35, SYSUTCDATETIME())),
(8, 5, 22, 'StartExam', 'StudentExamAttempt', '1', N'Started exam attempt', DATEADD(DAY, -30, SYSUTCDATETIME())),
(9, 5, 22, 'SubmitExam', 'StudentExamAttempt', '1', N'Submitted exam', DATEADD(DAY, -30, SYSUTCDATETIME())),
(10, 2, 22, 'GradeExam', 'StudentExamAttempt', '1', N'Graded exam - Score: 8.5', DATEADD(DAY, -29, SYSUTCDATETIME())),
(11, 3, 22, 'CreateCourse', 'Course', '3', N'Created AI course', DATEADD(MONTH, -8, SYSUTCDATETIME())),
(12, 3, 22, 'CreateClass', 'Class', '4', N'Created AI class', DATEADD(MONTH, -8, SYSUTCDATETIME())),
(13, 5, 22, 'CommentResource', 'Comment', '1', N'Commented on resource', DATEADD(DAY, -25, SYSUTCDATETIME())),
(14, 2, 22, 'Login', 'User', '2', N'User logged in', DATEADD(HOUR, -2, SYSUTCDATETIME())),
(15, 5, 22, 'Login', 'User', '5', N'User logged in', DATEADD(HOUR, -1, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.AuditLog OFF;
GO

-- ============================================================
-- SUBSCRIPTION & PAYMENT
-- ============================================================
SET IDENTITY_INSERT dbo.Subscription ON;

INSERT INTO dbo.Subscription (SubscriptionId, OrganizationId, PlanCode, Seats, StartAt, EndAt, Status, CreatedAt) VALUES
(1, 22, 'ENTERPRISE', 500, DATEADD(MONTH, -12, SYSUTCDATETIME()), DATEADD(MONTH, 12, SYSUTCDATETIME()), 'Active', DATEADD(MONTH, -12, SYSUTCDATETIME())),
(2, 33, 'PROFESSIONAL', 200, DATEADD(MONTH, -10, SYSUTCDATETIME()), DATEADD(MONTH, 14, SYSUTCDATETIME()), 'Active', DATEADD(MONTH, -10, SYSUTCDATETIME())),
(3, 44, 'PROFESSIONAL', 300, DATEADD(MONTH, -8, SYSUTCDATETIME()), DATEADD(MONTH, 16, SYSUTCDATETIME()), 'Active', DATEADD(MONTH, -8, SYSUTCDATETIME())),
(4, 55, 'BASIC', 100, DATEADD(MONTH, -6, SYSUTCDATETIME()), DATEADD(MONTH, 18, SYSUTCDATETIME()), 'Active', DATEADD(MONTH, -6, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.Subscription OFF;
GO

SET IDENTITY_INSERT dbo.Payment ON;

INSERT INTO dbo.Payment (PaymentId, OrganizationId, Amount, Currency, Provider, ProviderTransactionId, Status, CreatedAt) VALUES
(1, 22, 50000000.00, 'VND', 'VNPay', 'VNP_TXN_20240101_001', 'Completed', DATEADD(MONTH, -12, SYSUTCDATETIME())),
(2, 33, 30000000.00, 'VND', 'MoMo', 'MOMO_TXN_20240301_001', 'Completed', DATEADD(MONTH, -10, SYSUTCDATETIME())),
(3, 44, 35000000.00, 'VND', 'VNPay', 'VNP_TXN_20240501_001', 'Completed', DATEADD(MONTH, -8, SYSUTCDATETIME())),
(4, 55, 15000000.00, 'VND', 'VNPay', 'VNP_TXN_20240701_001', 'Completed', DATEADD(MONTH, -6, SYSUTCDATETIME())),
(5, 22, 50000000.00, 'VND', 'VNPay', 'VNP_TXN_20250101_001', 'Completed', DATEADD(MONTH, -1, SYSUTCDATETIME())),
(6, 33, 30000000.00, 'VND', 'MoMo', 'MOMO_TXN_20250115_001', 'Pending', DATEADD(DAY, -3, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.Payment OFF;
GO

-- ============================================================
-- FOCUS LOG (Anti-cheat tracking)
-- ============================================================
SET IDENTITY_INSERT dbo.FocusLog ON;

INSERT INTO dbo.FocusLog (FocusLogId, AttemptId, UserId, OccurredAt, DurationSeconds, WindowTitle, Details) VALUES
-- Attempt 1 focus losses
(1, 1, 5, DATEADD(MINUTE, -50, DATEADD(HOUR, -719, SYSUTCDATETIME())), 5, 'Facebook - Notifications', N'User switched to social media'),
(2, 1, 5, DATEADD(MINUTE, -30, DATEADD(HOUR, -719, SYSUTCDATETIME())), 8, 'WhatsApp Web', N'Checked messages'),

-- Attempt 4 focus losses
(3, 4, 8, DATEADD(MINUTE, -45, DATEADD(HOUR, -719, SYSUTCDATETIME())), 12, 'YouTube - Music', N'Opened YouTube'),
(4, 4, 8, DATEADD(MINUTE, -35, DATEADD(HOUR, -719, SYSUTCDATETIME())), 7, 'Gmail', N'Checked email'),
(5, 4, 8, DATEADD(MINUTE, -20, DATEADD(HOUR, -719, SYSUTCDATETIME())), 15, 'Google Search', N'Searched for answers'),

-- Attempt 8 focus losses
(6, 8, 8, DATEADD(MINUTE, -60, DATEADD(HOUR, -238, SYSUTCDATETIME())), 10, 'Messenger', N'Chatting'),
(7, 8, 8, DATEADD(MINUTE, -40, DATEADD(HOUR, -238, SYSUTCDATETIME())), 20, 'StackOverflow', N'Looking up code examples'),

-- Attempt 10 focus loss
(8, 10, 7, DATEADD(MINUTE, -25, DATEADD(HOUR, -600, SYSUTCDATETIME())), 3, 'Notification Center', N'Checked notification'),

-- Attempt 12 focus loss
(9, 12, 5, DATEADD(MINUTE, -40, DATEADD(HOUR, -479, SYSUTCDATETIME())), 6, 'Calendar', N'Checked schedule'),

-- Attempt 14 focus losses
(10, 14, 8, DATEADD(MINUTE, -55, DATEADD(HOUR, -479, SYSUTCDATETIME())), 18, 'Discord', N'Chatting with friends'),
(11, 14, 8, DATEADD(MINUTE, -30, DATEADD(HOUR, -479, SYSUTCDATETIME())), 9, 'Zalo', N'Group chat'),

-- Attempt 17 focus loss
(12, 17, 5, DATEADD(MINUTE, -60, DATEADD(HOUR, -118, SYSUTCDATETIME())), 4, 'Notes', N'Quick note taking'),

-- Attempt 19 focus loss
(13, 19, 12, DATEADD(MINUTE, -45, DATEADD(HOUR, -527, SYSUTCDATETIME())), 7, 'Calculator', N'Using calculator');

SET IDENTITY_INSERT dbo.FocusLog OFF;
GO

-- ============================================================
-- WEBCAM CAPTURE (Anti-cheat)
-- ============================================================
SET IDENTITY_INSERT dbo.WebcamCapture ON;

INSERT INTO dbo.WebcamCapture (CaptureId, AttemptId, UserId, FileId, CapturedAt, FaceDetected, MatchScore, Notes) VALUES
(1, 1, 5, NULL, DATEADD(MINUTE, -55, DATEADD(HOUR, -719, SYSUTCDATETIME())), 1, 95.50, N'Face matched'),
(2, 1, 5, NULL, DATEADD(MINUTE, -30, DATEADD(HOUR, -719, SYSUTCDATETIME())), 1, 94.20, N'Face matched'),
(3, 1, 5, NULL, DATEADD(MINUTE, -5, DATEADD(HOUR, -719, SYSUTCDATETIME())), 1, 96.10, N'Face matched'),
(4, 2, 6, NULL, DATEADD(MINUTE, -50, DATEADD(HOUR, -719, SYSUTCDATETIME())), 1, 92.30, N'Face matched'),
(5, 2, 6, NULL, DATEADD(MINUTE, -25, DATEADD(HOUR, -719, SYSUTCDATETIME())), 1, 93.80, N'Face matched'),
(6, 4, 8, NULL, DATEADD(MINUTE, -55, DATEADD(HOUR, -719, SYSUTCDATETIME())), 1, 88.50, N'Face matched with lower confidence'),
(7, 4, 8, NULL, DATEADD(MINUTE, -30, DATEADD(HOUR, -719, SYSUTCDATETIME())), 0, NULL, N'No face detected - suspicious'),
(8, 4, 8, NULL, DATEADD(MINUTE, -10, DATEADD(HOUR, -719, SYSUTCDATETIME())), 1, 90.20, N'Face detected again'),
(9, 17, 5, NULL, DATEADD(MINUTE, -80, DATEADD(HOUR, -118, SYSUTCDATETIME())), 1, 96.70, N'Face matched'),
(10, 17, 5, NULL, DATEADD(MINUTE, -40, DATEADD(HOUR, -118, SYSUTCDATETIME())), 1, 95.30, N'Face matched');

SET IDENTITY_INSERT dbo.WebcamCapture OFF;
GO

-- ============================================================
-- CHEATING ALERTS
-- ============================================================
SET IDENTITY_INSERT dbo.CheatingAlert ON;

INSERT INTO dbo.CheatingAlert (CheatingAlertId, AttemptId, UserId, AlertType, Severity, Details, CreatedAt, HandledBy, HandledAt) VALUES
(1, 4, 8, 'MultipleTabSwitch', 3, N'Switched tabs 3 times during exam', DATEADD(HOUR, -719, SYSUTCDATETIME()), 2, DATEADD(HOUR, -700, SYSUTCDATETIME())),
(2, 4, 8, 'NoFaceDetected', 4, N'Face not detected for 30 seconds', DATEADD(HOUR, -719, SYSUTCDATETIME()), 2, DATEADD(HOUR, -700, SYSUTCDATETIME())),
(3, 4, 8, 'SuspiciousSearch', 5, N'Opened Google Search and StackOverflow', DATEADD(HOUR, -719, SYSUTCDATETIME()), 2, DATEADD(HOUR, -700, SYSUTCDATETIME())),
(4, 8, 8, 'TabSwitch', 2, N'Switched to StackOverflow for 20 seconds', DATEADD(HOUR, -238, SYSUTCDATETIME()), NULL, NULL),
(5, 14, 8, 'ExcessiveFocusLoss', 3, N'Lost focus multiple times - 2 instances', DATEADD(HOUR, -479, SYSUTCDATETIME()), NULL, NULL);

SET IDENTITY_INSERT dbo.CheatingAlert OFF;
GO

-- ============================================================
-- EXTERNAL LOGIN (OAuth)
-- ============================================================
SET IDENTITY_INSERT dbo.ExternalLogin ON;

INSERT INTO dbo.ExternalLogin (ExternalLoginId, UserId, Provider, ProviderKey, DisplayName, ConnectedAt) VALUES
(1, 5, 'Google', 'google_109876543210', 'Phạm Minh Đức', DATEADD(MONTH, -8, SYSUTCDATETIME())),
(2, 6, 'Google', 'google_109876543211', 'Hoàng Thị Mai', DATEADD(MONTH, -8, SYSUTCDATETIME())),
(3, 7, 'Microsoft', 'microsoft_abc123def456', 'Trần Quốc Anh', DATEADD(MONTH, -7, SYSUTCDATETIME())),
(4, 9, 'Google', 'google_109876543212', 'Nguyễn Văn Khoa', DATEADD(MONTH, -6, SYSUTCDATETIME())),
(5, 12, 'Facebook', 'facebook_987654321', 'Đặng Minh Tuấn', DATEADD(MONTH, -5, SYSUTCDATETIME()));

SET IDENTITY_INSERT dbo.ExternalLogin OFF;
GO

-- ============================================================
-- REFRESH TOKENS (JWT)
-- ============================================================
SET IDENTITY_INSERT dbo.RefreshToken ON;

INSERT INTO dbo.RefreshToken (RefreshTokenId, UserId, Token, ExpiresAt, CreatedAt, CreatedByIp, RevokedAt, RevokedByIp, ReplacedByToken, ReasonRevoked) VALUES
-- Active tokens
(1, 5, 'RTK_5_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, 7, SYSUTCDATETIME()), DATEADD(HOUR, -2, SYSUTCDATETIME()), '192.168.1.10', NULL, NULL, NULL, NULL),
(2, 2, 'RTK_2_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, 7, SYSUTCDATETIME()), DATEADD(HOUR, -1, SYSUTCDATETIME()), '192.168.1.100', NULL, NULL, NULL, NULL),
(3, 7, 'RTK_7_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, 7, SYSUTCDATETIME()), DATEADD(HOUR, -3, SYSUTCDATETIME()), '192.168.1.12', NULL, NULL, NULL, NULL),

-- Revoked tokens
(4, 5, 'RTK_5_OLD_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, -1, SYSUTCDATETIME()), DATEADD(DAY, -8, SYSUTCDATETIME()), '192.168.1.10', DATEADD(DAY, -1, SYSUTCDATETIME()), '192.168.1.10', 'RTK_5_' + CONVERT(VARCHAR(50), NEWID()), 'Replaced by new token'),
(5, 6, 'RTK_6_REVOKED_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, 5, SYSUTCDATETIME()), DATEADD(DAY, -2, SYSUTCDATETIME()), '192.168.1.11', DATEADD(HOUR, -5, SYSUTCDATETIME()), '192.168.1.11', NULL, 'User logged out'),

-- Expired tokens
(6, 8, 'RTK_8_EXPIRED_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, -5, SYSUTCDATETIME()), DATEADD(DAY, -12, SYSUTCDATETIME()), '192.168.1.13', NULL, NULL, NULL, NULL);

SET IDENTITY_INSERT dbo.RefreshToken OFF;
GO

-- ============================================================
-- EMAIL CONFIRMATION TOKENS
-- ============================================================
SET IDENTITY_INSERT dbo.EmailConfirmationToken ON;

INSERT INTO dbo.EmailConfirmationToken (TokenId, UserId, Token, ExpiresAt, CreatedAt, UsedAt) VALUES
-- Used tokens (confirmed emails)
(1, 2, 'ECT_2_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, 1, DATEADD(MONTH, -11, SYSUTCDATETIME())), DATEADD(MONTH, -11, SYSUTCDATETIME()), DATEADD(HOUR, 2, DATEADD(MONTH, -11, SYSUTCDATETIME()))),
(2, 3, 'ECT_3_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, 1, DATEADD(MONTH, -10, SYSUTCDATETIME())), DATEADD(MONTH, -10, SYSUTCDATETIME()), DATEADD(HOUR, 1, DATEADD(MONTH, -10, SYSUTCDATETIME()))),
(3, 5, 'ECT_5_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, 1, DATEADD(MONTH, -8, SYSUTCDATETIME())), DATEADD(MONTH, -8, SYSUTCDATETIME()), DATEADD(HOUR, 3, DATEADD(MONTH, -8, SYSUTCDATETIME()))),
(4, 6, 'ECT_6_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, 1, DATEADD(MONTH, -8, SYSUTCDATETIME())), DATEADD(MONTH, -8, SYSUTCDATETIME()), DATEADD(HOUR, 5, DATEADD(MONTH, -8, SYSUTCDATETIME()))),

-- Expired unused token
(5, 17, 'ECT_17_EXPIRED_' + CONVERT(VARCHAR(50), NEWID()), DATEADD(DAY, -2, SYSUTCDATETIME()), DATEADD(DAY, -3, SYSUTCDATETIME()), NULL);

SET IDENTITY_INSERT dbo.EmailConfirmationToken OFF;
GO

-- ============================================================
-- UPDATE LastLoginAt FOR ACTIVE USERS
-- ============================================================
UPDATE dbo.[User] SET LastLoginAt = DATEADD(HOUR, -2, SYSUTCDATETIME()) WHERE UserId = 5;
UPDATE dbo.[User] SET LastLoginAt = DATEADD(HOUR, -1, SYSUTCDATETIME()) WHERE UserId = 2;
UPDATE dbo.[User] SET LastLoginAt = DATEADD(HOUR, -3, SYSUTCDATETIME()) WHERE UserId = 7;
UPDATE dbo.[User] SET LastLoginAt = DATEADD(DAY, -1, SYSUTCDATETIME()) WHERE UserId = 6;
UPDATE dbo.[User] SET LastLoginAt = DATEADD(DAY, -2, SYSUTCDATETIME()) WHERE UserId = 8;
UPDATE dbo.[User] SET LastLoginAt = DATEADD(HOUR, -5, SYSUTCDATETIME()) WHERE UserId = 3;
UPDATE dbo.[User] SET LastLoginAt = DATEADD(DAY, -3, SYSUTCDATETIME()) WHERE UserId = 12;
GO

-- ============================================================
-- VERIFICATION QUERIES
-- ============================================================

PRINT '=== DATASET SUMMARY ===';

DECLARE @Count INT;

SELECT @Count = COUNT(*) FROM dbo.Organization;
PRINT 'Organizations: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.[User];
PRINT 'Users: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.Course;
PRINT 'Courses: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.Class;
PRINT 'Classes: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.Enrollment;
PRINT 'Enrollments: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.Question;
PRINT 'Questions: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.Exam;
PRINT 'Exams: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.StudentExamAttempt;
PRINT 'Exam Attempts: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.StudentAnswer;
PRINT 'Student Answers: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.Resource;
PRINT 'Resources: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.Comment;
PRINT 'Comments: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.[Like];
PRINT 'Likes: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.Notification;
PRINT 'Notifications: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.AuditLog;
PRINT 'Audit Logs: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.Subscription;
PRINT 'Subscriptions: ' + CAST(@Count AS VARCHAR(10));

SELECT @Count = COUNT(*) FROM dbo.Payment;
PRINT 'Payments: ' + CAST(@Count AS VARCHAR(10));

PRINT '';
PRINT '=== SAMPLE DATA INSERTED SUCCESSFULLY ===';
GO
