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

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassMember> ClassMembers { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<StudentAnswer> StudentAnswers { get; set; }

    public virtual DbSet<StudentExamAttempt> StudentExamAttempts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwClassSummary> VwClassSummaries { get; set; }

    public virtual DbSet<VwExamResult> VwExamResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927C0D00DA1BE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Classes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classes__Created__4D94879B");
        });

        modelBuilder.Entity<ClassMember>(entity =>
        {
            entity.HasKey(e => e.ClassMemberId).HasName("PK__ClassMem__4205F71848C52F6B");

            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassMembers).HasConstraintName("FK__ClassMemb__Class__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.ClassMembers).HasConstraintName("FK__ClassMemb__UserI__5441852A");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exams__297521C72A37CA88");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.Property(e => e.TotalMarks).HasDefaultValue(100m);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Class).WithMany(p => p.Exams).HasConstraintName("FK__Exams__ClassId__6C190EBB");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exams__CreatedBy__6D0D32F4");
        });

        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.HasKey(e => e.ExamQuestionId).HasName("PK__ExamQues__EFAED846A6939870");

            entity.Property(e => e.Marks).HasDefaultValue(1m);

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamQuestions).HasConstraintName("FK__ExamQuest__ExamI__71D1E811");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamQuestions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamQuest__Quest__72C60C4A");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__C50610F78362E795");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Class).WithMany(p => p.Materials).HasConstraintName("FK__Materials__Class__59FA5E80");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Materials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Materials__Uploa__5AEE82B9");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.ResetTokenId).HasName("PK__Password__AF49A7B89C6DFDB4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens).HasConstraintName("FK__PasswordR__UserI__46E78A0C");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06FAC6A90DDF6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.QuestionType).HasDefaultValue("MultipleChoice");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Questions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Questions__Teach__619B8048");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.OptionId).HasName("PK__Question__92C7A1FFCF6E2E89");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionOptions).HasConstraintName("FK__QuestionO__Quest__656C112C");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__RefreshT__658FEEEA06C4543F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsRevoked).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens).HasConstraintName("FK__RefreshTo__UserI__412EB0B6");
        });

        modelBuilder.Entity<StudentAnswer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__StudentA__D482500496E50D6B");

            entity.Property(e => e.AnsweredAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Attempt).WithMany(p => p.StudentAnswers).HasConstraintName("FK__StudentAn__Attem__7D439ABD");

            entity.HasOne(d => d.Question).WithMany(p => p.StudentAnswers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentAn__Quest__7E37BEF6");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.StudentAnswers).HasConstraintName("FK__StudentAn__Selec__7F2BE32F");
        });

        modelBuilder.Entity<StudentExamAttempt>(entity =>
        {
            entity.HasKey(e => e.AttemptId).HasName("PK__StudentE__891A68E622818DB3");

            entity.Property(e => e.StartTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("InProgress");

            entity.HasOne(d => d.Exam).WithMany(p => p.StudentExamAttempts).HasConstraintName("FK__StudentEx__ExamI__787EE5A0");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentExamAttempts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEx__Stude__797309D9");
            // Thêm unique constraint vào StudentExamAttempts
            entity.HasIndex(e => new { e.ExamId, e.StudentId }).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CD01C7BAB");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<VwClassSummary>(entity =>
        {
            entity.ToView("vw_ClassSummary");
        });

        modelBuilder.Entity<VwExamResult>(entity =>
        {
            entity.ToView("vw_ExamResults");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}