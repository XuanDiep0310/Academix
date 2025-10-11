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