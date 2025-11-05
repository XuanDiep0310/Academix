using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Academix.Domain.Entities;

namespace Academix.Infrastructure;

public partial class AcademixDbContext : DbContext
{
    public AcademixDbContext()
    {
    }

    public AcademixDbContext(DbContextOptions<AcademixDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<CheatingAlert> CheatingAlerts { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }

    public virtual DbSet<ExternalLogin> ExternalLogins { get; set; }

    public virtual DbSet<FileStorage> FileStorages { get; set; }

    public virtual DbSet<FocusLog> FocusLogs { get; set; }

    public virtual DbSet<GamificationPoint> GamificationPoints { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Progress> Progresses { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

    public virtual DbSet<QuestionType> QuestionTypes { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<ResourceVersion> ResourceVersions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<StudentAnswer> StudentAnswers { get; set; }

    public virtual DbSet<StudentExamAttempt> StudentExamAttempts { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<TokenBlacklist> TokenBlacklists { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<VwExamAttemptLocalTime> VwExamAttemptLocalTimes { get; set; }

    public virtual DbSet<VwUserLocalTime> VwUserLocalTimes { get; set; }

    public virtual DbSet<WebcamCapture> WebcamCaptures { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F239833EB689C");

            entity.ToTable("AuditLog");

            entity.HasIndex(e => new { e.EntityType, e.EntityId }, "IX_AuditLog_Entity");

            entity.HasIndex(e => e.CreatedAt, "IX_AuditLog_Time");

            entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "IX_AuditLog_User");

            entity.Property(e => e.Action).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EntityId).HasMaxLength(200);
            entity.Property(e => e.EntityType).HasMaxLength(200);

            entity.HasOne(d => d.Organization).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__AuditLog__Organi__7E02B4CC");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AuditLog__UserId__7D0E9093");
        });

        modelBuilder.Entity<CheatingAlert>(entity =>
        {
            entity.HasKey(e => e.CheatingAlertId).HasName("PK__Cheating__990ABDF80D30E7B9");

            entity.ToTable("CheatingAlert");

            entity.HasIndex(e => e.AttemptId, "IX_CheatingAlert_Attempt");

            entity.HasIndex(e => e.HandledAt, "IX_CheatingAlert_Status");

            entity.HasIndex(e => e.UserId, "IX_CheatingAlert_User");

            entity.Property(e => e.AlertType).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Severity).HasDefaultValue((byte)1);

            entity.HasOne(d => d.Attempt).WithMany(p => p.CheatingAlerts)
                .HasForeignKey(d => d.AttemptId)
                .HasConstraintName("FK__CheatingA__Attem__6166761E");

            entity.HasOne(d => d.HandledByNavigation).WithMany(p => p.CheatingAlertHandledByNavigations)
                .HasForeignKey(d => d.HandledBy)
                .HasConstraintName("FK__CheatingA__Handl__65370702");

            entity.HasOne(d => d.User).WithMany(p => p.CheatingAlertUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CheatingA__UserI__625A9A57");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__CB1927C084160A01");

            entity.ToTable("Class");

            entity.HasIndex(e => e.IsActive, "IX_Class_Active").HasFilter("([IsActive]=(1))");

            entity.HasIndex(e => e.CourseId, "IX_Class_Course");

            entity.HasIndex(e => e.TeacherId, "IX_Class_Teacher");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EnrollmentCode).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(300);

            entity.HasOne(d => d.Course).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Class__CourseId__6383C8BA");

            entity.HasOne(d => d.Organization).WithMany(p => p.Classes)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__Class__Organizat__6477ECF3");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__Class__TeacherId__656C112C");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__C3B4DFCAB850018F");

            entity.ToTable("Comment");

            entity.HasIndex(e => new { e.EntityType, e.EntityId }, "IX_Comment_Entity");

            entity.HasIndex(e => e.ParentCommentId, "IX_Comment_Parent");

            entity.HasIndex(e => e.UserId, "IX_Comment_User");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EntityType).HasMaxLength(100);

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK__Comment__ParentC__0E6E26BF");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__UserId__0F624AF8");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D71A751721422");

            entity.ToTable("Course");

            entity.HasIndex(e => e.IsPublished, "IX_Course_Published").HasFilter("([IsPublished]=(1))");

            entity.HasIndex(e => new { e.OrganizationId, e.Code }, "UQ_Course_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ShortDescription).HasMaxLength(1000);
            entity.Property(e => e.Title).HasMaxLength(300);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Course__CreatedB__5EBF139D");

            entity.HasOne(d => d.Organization).WithMany(p => p.Courses)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__Course__Organiza__5DCAEF64");
        });

        modelBuilder.Entity<EmailConfirmationToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__EmailCon__658FEEEAA94EA693");

            entity.ToTable("EmailConfirmationToken");

            entity.HasIndex(e => e.Token, "IX_EmailConfirmationToken_Token").HasFilter("([UsedAt] IS NULL)");

            entity.HasIndex(e => e.UserId, "IX_EmailConfirmationToken_User");

            entity.HasIndex(e => e.Token, "UQ__EmailCon__1EB4F817A4A2FBC9").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsExpired).HasComputedColumnSql("(case when [ExpiresAt]<sysutcdatetime() then (1) else (0) end)", false);
            entity.Property(e => e.IsUsed).HasComputedColumnSql("(case when [UsedAt] IS NOT NULL then (1) else (0) end)", false);
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.EmailConfirmationTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__EmailConf__UserI__2704CA5F");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F68771B6432CDE1");

            entity.ToTable("Enrollment");

            entity.HasIndex(e => new { e.ClassId, e.UserId, e.IsActive }, "IX_Enrollment_Active");

            entity.HasIndex(e => e.ClassId, "IX_Enrollment_Class");

            entity.HasIndex(e => e.UserId, "IX_Enrollment_User");

            entity.HasIndex(e => new { e.ClassId, e.UserId }, "UQ_Enrollment").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsApproved).HasDefaultValue(true);
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RoleInClass)
                .HasMaxLength(50)
                .HasDefaultValue("Student");

            entity.HasOne(d => d.Class).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Class__6D0D32F4");

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__UserI__6E01572D");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exam__297521C77A0784DB");

            entity.ToTable("Exam");

            entity.HasIndex(e => e.ClassId, "IX_Exam_Class");

            entity.HasIndex(e => e.CourseId, "IX_Exam_Course");

            entity.HasIndex(e => new { e.StartAt, e.EndAt }, "IX_Exam_Schedule");

            entity.Property(e => e.AllowBackNavigation).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Title).HasMaxLength(300);

            entity.HasOne(d => d.Class).WithMany(p => p.Exams)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Exam__ClassId__2BFE89A6");

            entity.HasOne(d => d.Course).WithMany(p => p.Exams)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Exam__CourseId__2B0A656D");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Exam__CreatedBy__30C33EC3");

            entity.HasOne(d => d.Organization).WithMany(p => p.Exams)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__Exam__Organizati__2A164134");
        });

        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.HasKey(e => e.ExamQuestionId).HasName("PK__ExamQues__EFAED84657C94938");

            entity.ToTable("ExamQuestion");

            entity.HasIndex(e => new { e.ExamId, e.OrderIndex }, "IX_ExamQuestion_Exam");

            entity.HasIndex(e => new { e.ExamId, e.QuestionId }, "UQ_ExamQuestion").IsUnique();

            entity.Property(e => e.Score)
                .HasDefaultValue(10m)
                .HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("FK__ExamQuest__ExamI__3864608B");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamQuest__Quest__395884C4");
        });

        modelBuilder.Entity<ExternalLogin>(entity =>
        {
            entity.HasKey(e => e.ExternalLoginId).HasName("PK__External__A8FDB3AE1F1DC54A");

            entity.ToTable("ExternalLogin");

            entity.HasIndex(e => e.UserId, "IX_ExternalLogin_User");

            entity.HasIndex(e => new { e.Provider, e.ProviderKey }, "UQ_ExternalLogin").IsUnique();

            entity.Property(e => e.ConnectedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Provider).HasMaxLength(100);
            entity.Property(e => e.ProviderKey).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.ExternalLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ExternalL__UserI__59063A47");
        });

        modelBuilder.Entity<FileStorage>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__FileStor__6F0F98BF198CA379");

            entity.ToTable("FileStorage");

            entity.HasIndex(e => e.OrganizationId, "IX_FileStorage_Org");

            entity.HasIndex(e => e.UploadedBy, "IX_FileStorage_Uploaded");

            entity.Property(e => e.BlobUri).HasMaxLength(2000);
            entity.Property(e => e.Checksum).HasMaxLength(128);
            entity.Property(e => e.ContentType).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FileName).HasMaxLength(500);
            entity.Property(e => e.StorageProvider).HasMaxLength(100);

            entity.HasOne(d => d.Organization).WithMany(p => p.FileStorages)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__FileStora__Organ__75A278F5");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.FileStorages)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK__FileStora__Uploa__76969D2E");
        });

        modelBuilder.Entity<FocusLog>(entity =>
        {
            entity.HasKey(e => e.FocusLogId).HasName("PK__FocusLog__8429940395F35C27");

            entity.ToTable("FocusLog");

            entity.HasIndex(e => new { e.AttemptId, e.OccurredAt }, "IX_FocusLog_Attempt");

            entity.HasIndex(e => e.UserId, "IX_FocusLog_User");

            entity.Property(e => e.OccurredAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WindowTitle).HasMaxLength(1000);

            entity.HasOne(d => d.Attempt).WithMany(p => p.FocusLogs)
                .HasForeignKey(d => d.AttemptId)
                .HasConstraintName("FK__FocusLog__Attemp__540C7B00");

            entity.HasOne(d => d.User).WithMany(p => p.FocusLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FocusLog__UserId__55009F39");
        });

        modelBuilder.Entity<GamificationPoint>(entity =>
        {
            entity.HasKey(e => e.PointId).HasName("PK__Gamifica__40A977E1F566BF2F");

            entity.ToTable("GamificationPoint");

            entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "IX_GamificationPoint_User");

            entity.Property(e => e.ActivityType).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RelatedEntityType).HasMaxLength(100);

            entity.HasOne(d => d.Organization).WithMany(p => p.GamificationPoints)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__Gamificat__Organ__793DFFAF");

            entity.HasOne(d => d.User).WithMany(p => p.GamificationPoints)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Gamificat__UserI__7849DB76");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__Like__A2922C142270CC3B");

            entity.ToTable("Like");

            entity.HasIndex(e => new { e.EntityType, e.EntityId }, "IX_Like_Entity");

            entity.HasIndex(e => e.UserId, "IX_Like_User");

            entity.HasIndex(e => new { e.EntityType, e.EntityId, e.UserId }, "UQ_Like").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EntityType).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Like__UserId__160F4887");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12D5D3F310");

            entity.ToTable("Notification");

            entity.HasIndex(e => new { e.UserId, e.IsRead, e.CreatedAt }, "IX_Notification_User");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.Type).HasMaxLength(100);

            entity.HasOne(d => d.Organization).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__Notificat__Organ__690797E6");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__69FBBC1F");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("PK__Organiza__CADB0B12B7F9A5DD");

            entity.ToTable("Organization");

            entity.Property(e => e.BillingContact).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Domain).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LogoUrl).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(250);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A389850126F");

            entity.ToTable("Payment");

            entity.HasIndex(e => new { e.OrganizationId, e.CreatedAt }, "IX_Payment_Org");

            entity.HasIndex(e => new { e.Provider, e.ProviderTransactionId }, "IX_Payment_Provider");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("VND");
            entity.Property(e => e.Provider).HasMaxLength(100);
            entity.Property(e => e.ProviderTransactionId).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Organization).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Organiz__09746778");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB2FD0D1B98B");

            entity.ToTable("Permission");

            entity.HasIndex(e => e.Code, "UQ__Permissi__A25C5AA76428817C").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Progress>(entity =>
        {
            entity.HasKey(e => e.ProgressId).HasName("PK__Progress__BAE29CA547DCB551");

            entity.ToTable("Progress");

            entity.HasIndex(e => e.ResourceId, "IX_Progress_Resource");

            entity.HasIndex(e => e.UserId, "IX_Progress_User");

            entity.HasIndex(e => new { e.UserId, e.ResourceId }, "UQ_Progress").IsUnique();

            entity.Property(e => e.LastSeenAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WatchedPercentage).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Resource).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Progress__Resour__719CDDE7");

            entity.HasOne(d => d.User).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Progress__UserId__70A8B9AE");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06FACEE36E131");

            entity.ToTable("Question");

            entity.HasIndex(e => e.IsDeleted, "IX_Question_Active").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.OrganizationId, "IX_Question_Org");

            entity.HasIndex(e => e.TypeId, "IX_Question_Type");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Question__Create__1DB06A4F");

            entity.HasOne(d => d.Organization).WithMany(p => p.Questions)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__Question__Organi__1CBC4616");

            entity.HasOne(d => d.Type).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Question__TypeId__1EA48E88");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.OptionId).HasName("PK__Question__92C7A1FFF659C629");

            entity.ToTable("QuestionOption");

            entity.HasIndex(e => e.QuestionId, "IX_QuestionOption_Question");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionOptions)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__QuestionO__Quest__245D67DE");
        });

        modelBuilder.Entity<QuestionType>(entity =>
        {
            entity.HasKey(e => e.QuestionTypeId).HasName("PK__Question__7EDFF931089B1682");

            entity.ToTable("QuestionType");

            entity.HasIndex(e => e.Name, "UQ__Question__737584F62577FA09").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("PK__RefreshT__F5845E393DC55364");

            entity.ToTable("RefreshToken");

            entity.HasIndex(e => e.Token, "IX_RefreshToken_Token").HasFilter("([RevokedAt] IS NULL)");

            entity.HasIndex(e => e.UserId, "IX_RefreshToken_User");

            entity.HasIndex(e => e.Token, "UQ__RefreshT__1EB4F8172DED076C").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CreatedByIp).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasComputedColumnSql("(case when [RevokedAt] IS NULL AND [ExpiresAt]>sysutcdatetime() then (1) else (0) end)", false);
            entity.Property(e => e.IsExpired).HasComputedColumnSql("(case when [ExpiresAt]<sysutcdatetime() then (1) else (0) end)", false);
            entity.Property(e => e.IsRevoked).HasComputedColumnSql("(case when [RevokedAt] IS NOT NULL then (1) else (0) end)", false);
            entity.Property(e => e.ReasonRevoked).HasMaxLength(200);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            entity.Property(e => e.RevokedByIp).HasMaxLength(50);
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RefreshTo__UserI__1D7B6025");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).HasName("PK__Resource__4ED1816F15479255");

            entity.ToTable("Resource");

            entity.HasIndex(e => e.IsDeleted, "IX_Resource_Active").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.ClassId, "IX_Resource_Class");

            entity.HasIndex(e => e.CourseId, "IX_Resource_Course");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ResourceType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.Visibility)
                .HasMaxLength(50)
                .HasDefaultValue("Public");

            entity.HasOne(d => d.Class).WithMany(p => p.Resources)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Resource__ClassI__7D439ABD");

            entity.HasOne(d => d.Course).WithMany(p => p.Resources)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Resource__Course__7C4F7684");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Resources)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Resource__Create__7F2BE32F");

            entity.HasOne(d => d.Organization).WithMany(p => p.Resources)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__Resource__Organi__7B5B524B");
        });

        modelBuilder.Entity<ResourceVersion>(entity =>
        {
            entity.HasKey(e => e.ResourceVersionId).HasName("PK__Resource__686BCB14E5E56F1D");

            entity.ToTable("ResourceVersion");

            entity.HasIndex(e => e.ResourceId, "IX_ResourceVersion_Resource");

            entity.HasIndex(e => new { e.ResourceId, e.VersionNumber }, "UQ_ResourceVersion").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.VersionNumber).HasDefaultValue(1);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ResourceVersions)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ResourceV__Creat__09A971A2");

            entity.HasOne(d => d.File).WithMany(p => p.ResourceVersions)
                .HasForeignKey(d => d.FileId)
                .HasConstraintName("FK__ResourceV__FileI__07C12930");

            entity.HasOne(d => d.Resource).WithMany(p => p.ResourceVersions)
                .HasForeignKey(d => d.ResourceId)
                .HasConstraintName("FK__ResourceV__Resou__06CD04F7");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1AD7AEC81D");

            entity.ToTable("Role");

            entity.HasIndex(e => new { e.OrganizationId, e.Name }, "UQ_Role_Name").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Organization).WithMany(p => p.Roles)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__Role__Organizati__46E78A0C");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("PK__RolePerm__120F46BAFCCD2B60");

            entity.ToTable("RolePermission");

            entity.HasIndex(e => e.RoleId, "IX_RolePermission_Role");

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }, "UQ_RolePermission").IsUnique();

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK__RolePermi__Permi__5535A963");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__RolePermi__RoleI__5441852A");
        });

        modelBuilder.Entity<StudentAnswer>(entity =>
        {
            entity.HasKey(e => e.StudentAnswerId).HasName("PK__StudentA__6E3EA405DBEE3FA3");

            entity.ToTable("StudentAnswer");

            entity.HasIndex(e => e.AttemptId, "IX_StudentAnswer_Attempt");

            entity.HasIndex(e => e.QuestionId, "IX_StudentAnswer_Question");

            entity.HasIndex(e => new { e.AttemptId, e.QuestionId }, "UQ_StudentAnswer").IsUnique();

            entity.Property(e => e.ScoreAwarded).HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.Attempt).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.AttemptId)
                .HasConstraintName("FK__StudentAn__Attem__4B7734FF");

            entity.HasOne(d => d.File).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.FileId)
                .HasConstraintName("FK__StudentAn__FileI__4E53A1AA");

            entity.HasOne(d => d.GradedByNavigation).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.GradedBy)
                .HasConstraintName("FK__StudentAn__Grade__503BEA1C");

            entity.HasOne(d => d.Question).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentAn__Quest__4C6B5938");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.SelectedOptionId)
                .HasConstraintName("FK__StudentAn__Selec__4D5F7D71");
        });

        modelBuilder.Entity<StudentExamAttempt>(entity =>
        {
            entity.HasKey(e => e.AttemptId).HasName("PK__StudentE__891A68E630F288BA");

            entity.ToTable("StudentExamAttempt");

            entity.HasIndex(e => e.ExamId, "IX_Attempt_Exam");

            entity.HasIndex(e => e.Status, "IX_Attempt_Status");

            entity.HasIndex(e => new { e.StartedAt, e.SubmittedAt }, "IX_Attempt_Time");

            entity.HasIndex(e => e.UserId, "IX_Attempt_User");

            entity.Property(e => e.Ipaddress)
                .HasMaxLength(100)
                .HasColumnName("IPAddress");
            entity.Property(e => e.Score).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.StartedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("InProgress");

            entity.HasOne(d => d.Class).WithMany(p => p.StudentExamAttempts)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__StudentEx__Class__41EDCAC5");

            entity.HasOne(d => d.Exam).WithMany(p => p.StudentExamAttempts)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEx__ExamI__40058253");

            entity.HasOne(d => d.User).WithMany(p => p.StudentExamAttempts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEx__UserI__40F9A68C");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Subscrip__9A2B249DBAABAF8E");

            entity.ToTable("Subscription");

            entity.HasIndex(e => new { e.OrganizationId, e.Status }, "IX_Subscription_Org");

            entity.HasIndex(e => new { e.StartAt, e.EndAt }, "IX_Subscription_Period");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.PlanCode).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Organization).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__Organ__01D345B0");
        });

        modelBuilder.Entity<TokenBlacklist>(entity =>
        {
            entity.HasKey(e => e.TokenBlacklistId).HasName("PK__TokenBla__1C7C765B06A409F2");

            entity.ToTable("TokenBlacklist");

            entity.HasIndex(e => e.ExpiresAt, "IX_TokenBlacklist_Expiry");

            entity.HasIndex(e => e.UserId, "IX_TokenBlacklist_User");

            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.RevokedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C977BC228");

            entity.ToTable("User");

            entity.HasIndex(e => new { e.IsActive, e.IsDeleted }, "IX_User_Active").HasFilter("([IsActive]=(1) AND [IsDeleted]=(0))");

            entity.HasIndex(e => new { e.OrganizationId, e.NormalizedEmail }, "IX_User_Email");

            entity.HasIndex(e => new { e.OrganizationId, e.NormalizedEmail }, "UQ_User_Email").IsUnique();

            entity.Property(e => e.AvatarUrl).HasMaxLength(1000);
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Lưu thời gian tạo theo UTC (Coordinated Universal Time). KHÔNG dùng giờ địa phương.");
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLoginAt).HasComment("Lưu lần login cuối theo UTC. Convert sang giờ địa phương khi hiển thị.");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.TwoFaenabled).HasColumnName("TwoFAEnabled");
            entity.Property(e => e.TwoFasecret)
                .HasMaxLength(512)
                .HasColumnName("TwoFASecret");
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Organization).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__User__Organizati__3D5E1FD2");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__3D978A357CEA2F5A");

            entity.ToTable("UserRole");

            entity.HasIndex(e => e.RoleId, "IX_UserRole_Role");

            entity.HasIndex(e => e.UserId, "IX_UserRole_User");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UQ_UserRole").IsUnique();

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__UserRole__RoleId__4CA06362");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserRole__UserId__4BAC3F29");
        });

        modelBuilder.Entity<VwExamAttemptLocalTime>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ExamAttempt_LocalTime");

            entity.Property(e => e.AttemptId).ValueGeneratedOnAdd();
            entity.Property(e => e.Score).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.StartedAtUtc).HasColumnName("StartedAtUTC");
            entity.Property(e => e.StartedAtVn).HasColumnName("StartedAtVN");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SubmittedAtUtc).HasColumnName("SubmittedAtUTC");
            entity.Property(e => e.SubmittedAtVn).HasColumnName("SubmittedAtVN");
        });

        modelBuilder.Entity<VwUserLocalTime>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_User_LocalTime");

            entity.Property(e => e.CreatedAtUtc).HasColumnName("CreatedAtUTC");
            entity.Property(e => e.CreatedAtVn).HasColumnName("CreatedAtVN");
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.LastLoginAtUtc).HasColumnName("LastLoginAtUTC");
            entity.Property(e => e.LastLoginAtVn).HasColumnName("LastLoginAtVN");
            entity.Property(e => e.UserId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<WebcamCapture>(entity =>
        {
            entity.HasKey(e => e.CaptureId).HasName("PK__WebcamCa__EE62873F4208E1D7");

            entity.ToTable("WebcamCapture");

            entity.HasIndex(e => new { e.AttemptId, e.CapturedAt }, "IX_WebcamCapture_Attempt");

            entity.Property(e => e.CapturedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MatchScore).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Attempt).WithMany(p => p.WebcamCaptures)
                .HasForeignKey(d => d.AttemptId)
                .HasConstraintName("FK__WebcamCap__Attem__59C55456");

            entity.HasOne(d => d.File).WithMany(p => p.WebcamCaptures)
                .HasForeignKey(d => d.FileId)
                .HasConstraintName("FK__WebcamCap__FileI__5BAD9CC8");

            entity.HasOne(d => d.User).WithMany(p => p.WebcamCaptures)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WebcamCap__UserI__5AB9788F");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__Password__658FEEEA82973FF8");

            entity.HasIndex(e => e.Token, "IX_PasswordResetToken_Token").HasFilter("([UsedAt] IS NULL)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsExpired).HasComputedColumnSql("(case when [ExpiresAt]<sysutcdatetime() then (1) else (0) end)", false);
            entity.Property(e => e.IsUsed).HasComputedColumnSql("(case when [UsedAt] IS NOT NULL then (1) else (0) end)", false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
