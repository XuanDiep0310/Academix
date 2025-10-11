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

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<ResourceVersion> ResourceVersions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<StudentAnswer> StudentAnswers { get; set; }

    public virtual DbSet<StudentExamAttempt> StudentExamAttempts { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<VwExamAttemptLocalTime> VwExamAttemptLocalTimes { get; set; }

    public virtual DbSet<VwUserLocalTime> VwUserLocalTimes { get; set; }

    public virtual DbSet<WebcamCapture> WebcamCaptures { get; set; }

    // This has all the schema definitions based on the tables and its configuration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F239833EB689C");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Organization).WithMany(p => p.AuditLogs).HasConstraintName("FK__AuditLog__Organi__7E02B4CC");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs).HasConstraintName("FK__AuditLog__UserId__7D0E9093");
        });

        modelBuilder.Entity<CheatingAlert>(entity =>
        {
            entity.HasKey(e => e.CheatingAlertId).HasName("PK__Cheating__990ABDF80D30E7B9");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Severity).HasDefaultValue((byte)1);

            entity.HasOne(d => d.Attempt).WithMany(p => p.CheatingAlerts).HasConstraintName("FK__CheatingA__Attem__6166761E");

            entity.HasOne(d => d.HandledByNavigation).WithMany(p => p.CheatingAlertHandledByNavigations).HasConstraintName("FK__CheatingA__Handl__65370702");

            entity.HasOne(d => d.User).WithMany(p => p.CheatingAlertUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CheatingA__UserI__625A9A57");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__CB1927C084160A01");

            entity.HasIndex(e => e.IsActive, "IX_Class_Active").HasFilter("([IsActive]=(1))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Course).WithMany(p => p.Classes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Class__CourseId__6383C8BA");

            entity.HasOne(d => d.Organization).WithMany(p => p.Classes).HasConstraintName("FK__Class__Organizat__6477ECF3");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Classes).HasConstraintName("FK__Class__TeacherId__656C112C");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__C3B4DFCAB850018F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment).HasConstraintName("FK__Comment__ParentC__0E6E26BF");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__UserId__0F624AF8");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D71A751721422");

            entity.HasIndex(e => e.IsPublished, "IX_Course_Published").HasFilter("([IsPublished]=(1))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Courses).HasConstraintName("FK__Course__CreatedB__5EBF139D");

            entity.HasOne(d => d.Organization).WithMany(p => p.Courses).HasConstraintName("FK__Course__Organiza__5DCAEF64");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F68771B6432CDE1");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsApproved).HasDefaultValue(true);
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RoleInClass).HasDefaultValue("Student");

            entity.HasOne(d => d.Class).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Class__6D0D32F4");

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__UserI__6E01572D");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exam__297521C77A0784DB");

            entity.Property(e => e.AllowBackNavigation).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Class).WithMany(p => p.Exams).HasConstraintName("FK__Exam__ClassId__2BFE89A6");

            entity.HasOne(d => d.Course).WithMany(p => p.Exams).HasConstraintName("FK__Exam__CourseId__2B0A656D");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Exams).HasConstraintName("FK__Exam__CreatedBy__30C33EC3");

            entity.HasOne(d => d.Organization).WithMany(p => p.Exams).HasConstraintName("FK__Exam__Organizati__2A164134");
        });

        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.HasKey(e => e.ExamQuestionId).HasName("PK__ExamQues__EFAED84657C94938");

            entity.Property(e => e.Score).HasDefaultValue(10m);

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamQuestions).HasConstraintName("FK__ExamQuest__ExamI__3864608B");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamQuestions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamQuest__Quest__395884C4");
        });

        modelBuilder.Entity<ExternalLogin>(entity =>
        {
            entity.HasKey(e => e.ExternalLoginId).HasName("PK__External__A8FDB3AE1F1DC54A");

            entity.Property(e => e.ConnectedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.ExternalLogins).HasConstraintName("FK__ExternalL__UserI__59063A47");
        });

        modelBuilder.Entity<FileStorage>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__FileStor__6F0F98BF198CA379");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Organization).WithMany(p => p.FileStorages).HasConstraintName("FK__FileStora__Organ__75A278F5");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.FileStorages).HasConstraintName("FK__FileStora__Uploa__76969D2E");
        });

        modelBuilder.Entity<FocusLog>(entity =>
        {
            entity.HasKey(e => e.FocusLogId).HasName("PK__FocusLog__8429940395F35C27");

            entity.Property(e => e.OccurredAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Attempt).WithMany(p => p.FocusLogs).HasConstraintName("FK__FocusLog__Attemp__540C7B00");

            entity.HasOne(d => d.User).WithMany(p => p.FocusLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FocusLog__UserId__55009F39");
        });

        modelBuilder.Entity<GamificationPoint>(entity =>
        {
            entity.HasKey(e => e.PointId).HasName("PK__Gamifica__40A977E1F566BF2F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Organization).WithMany(p => p.GamificationPoints).HasConstraintName("FK__Gamificat__Organ__793DFFAF");

            entity.HasOne(d => d.User).WithMany(p => p.GamificationPoints)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Gamificat__UserI__7849DB76");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__Like__A2922C142270CC3B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Like__UserId__160F4887");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12D5D3F310");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Organization).WithMany(p => p.Notifications).HasConstraintName("FK__Notificat__Organ__690797E6");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications).HasConstraintName("FK__Notificat__UserI__69FBBC1F");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("PK__Organiza__CADB0B12B7F9A5DD");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A389850126F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Currency).HasDefaultValue("VND");

            entity.HasOne(d => d.Organization).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Organiz__09746778");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB2FD0D1B98B");
        });

        modelBuilder.Entity<Progress>(entity =>
        {
            entity.HasKey(e => e.ProgressId).HasName("PK__Progress__BAE29CA547DCB551");

            entity.Property(e => e.LastSeenAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Resource).WithMany(p => p.Progresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Progress__Resour__719CDDE7");

            entity.HasOne(d => d.User).WithMany(p => p.Progresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Progress__UserId__70A8B9AE");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06FACEE36E131");

            entity.HasIndex(e => e.IsDeleted, "IX_Question_Active").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Questions).HasConstraintName("FK__Question__Create__1DB06A4F");

            entity.HasOne(d => d.Organization).WithMany(p => p.Questions).HasConstraintName("FK__Question__Organi__1CBC4616");

            entity.HasOne(d => d.Type).WithMany(p => p.Questions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Question__TypeId__1EA48E88");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.OptionId).HasName("PK__Question__92C7A1FFF659C629");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionOptions).HasConstraintName("FK__QuestionO__Quest__245D67DE");
        });

        modelBuilder.Entity<QuestionType>(entity =>
        {
            entity.HasKey(e => e.QuestionTypeId).HasName("PK__Question__7EDFF931089B1682");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).HasName("PK__Resource__4ED1816F15479255");

            entity.HasIndex(e => e.IsDeleted, "IX_Resource_Active").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Visibility).HasDefaultValue("Public");

            entity.HasOne(d => d.Class).WithMany(p => p.Resources).HasConstraintName("FK__Resource__ClassI__7D439ABD");

            entity.HasOne(d => d.Course).WithMany(p => p.Resources).HasConstraintName("FK__Resource__Course__7C4F7684");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Resources).HasConstraintName("FK__Resource__Create__7F2BE32F");

            entity.HasOne(d => d.Organization).WithMany(p => p.Resources).HasConstraintName("FK__Resource__Organi__7B5B524B");
        });

        modelBuilder.Entity<ResourceVersion>(entity =>
        {
            entity.HasKey(e => e.ResourceVersionId).HasName("PK__Resource__686BCB14E5E56F1D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.VersionNumber).HasDefaultValue(1);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ResourceVersions).HasConstraintName("FK__ResourceV__Creat__09A971A2");

            entity.HasOne(d => d.File).WithMany(p => p.ResourceVersions).HasConstraintName("FK__ResourceV__FileI__07C12930");

            entity.HasOne(d => d.Resource).WithMany(p => p.ResourceVersions).HasConstraintName("FK__ResourceV__Resou__06CD04F7");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1AD7AEC81D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Organization).WithMany(p => p.Roles).HasConstraintName("FK__Role__Organizati__46E78A0C");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("PK__RolePerm__120F46BAFCCD2B60");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions).HasConstraintName("FK__RolePermi__Permi__5535A963");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions).HasConstraintName("FK__RolePermi__RoleI__5441852A");
        });

        modelBuilder.Entity<StudentAnswer>(entity =>
        {
            entity.HasKey(e => e.StudentAnswerId).HasName("PK__StudentA__6E3EA405DBEE3FA3");

            entity.HasOne(d => d.Attempt).WithMany(p => p.StudentAnswers).HasConstraintName("FK__StudentAn__Attem__4B7734FF");

            entity.HasOne(d => d.File).WithMany(p => p.StudentAnswers).HasConstraintName("FK__StudentAn__FileI__4E53A1AA");

            entity.HasOne(d => d.GradedByNavigation).WithMany(p => p.StudentAnswers).HasConstraintName("FK__StudentAn__Grade__503BEA1C");

            entity.HasOne(d => d.Question).WithMany(p => p.StudentAnswers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentAn__Quest__4C6B5938");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.StudentAnswers).HasConstraintName("FK__StudentAn__Selec__4D5F7D71");
        });

        modelBuilder.Entity<StudentExamAttempt>(entity =>
        {
            entity.HasKey(e => e.AttemptId).HasName("PK__StudentE__891A68E630F288BA");

            entity.Property(e => e.StartedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue("InProgress");

            entity.HasOne(d => d.Class).WithMany(p => p.StudentExamAttempts).HasConstraintName("FK__StudentEx__Class__41EDCAC5");

            entity.HasOne(d => d.Exam).WithMany(p => p.StudentExamAttempts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEx__ExamI__40058253");

            entity.HasOne(d => d.User).WithMany(p => p.StudentExamAttempts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEx__UserI__40F9A68C");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Subscrip__9A2B249DBAABAF8E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Organization).WithMany(p => p.Subscriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__Organ__01D345B0");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C977BC228");

            entity.HasIndex(e => new { e.IsActive, e.IsDeleted }, "IX_User_Active").HasFilter("([IsActive]=(1) AND [IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Lưu thời gian tạo theo UTC (Coordinated Universal Time). KHÔNG dùng giờ địa phương.");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLoginAt).HasComment("Lưu lần login cuối theo UTC. Convert sang giờ địa phương khi hiển thị.");

            entity.HasOne(d => d.Organization).WithMany(p => p.Users).HasConstraintName("FK__User__Organizati__3D5E1FD2");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__3D978A357CEA2F5A");

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasConstraintName("FK__UserRole__RoleId__4CA06362");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasConstraintName("FK__UserRole__UserId__4BAC3F29");
        });

        modelBuilder.Entity<VwExamAttemptLocalTime>(entity =>
        {
            entity.ToView("vw_ExamAttempt_LocalTime");

            entity.Property(e => e.AttemptId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<VwUserLocalTime>(entity =>
        {
            entity.ToView("vw_User_LocalTime");

            entity.Property(e => e.UserId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<WebcamCapture>(entity =>
        {
            entity.HasKey(e => e.CaptureId).HasName("PK__WebcamCa__EE62873F4208E1D7");

            entity.Property(e => e.CapturedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Attempt).WithMany(p => p.WebcamCaptures).HasConstraintName("FK__WebcamCap__Attem__59C55456");

            entity.HasOne(d => d.File).WithMany(p => p.WebcamCaptures).HasConstraintName("FK__WebcamCap__FileI__5BAD9CC8");

            entity.HasOne(d => d.User).WithMany(p => p.WebcamCaptures)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WebcamCap__UserI__5AB9788F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
