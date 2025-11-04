using Academix.Application.DTOs.Teacher;
using Academix.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly AcademixDbContext _context;

        public TeacherService(AcademixDbContext context)
        {
            _context = context;
        }
        public async Task<TeacherDashboardDto> GetDashboardAsync(int teacherId)
        {
            var result = new TeacherDashboardDto();

            // 1️ Lấy lớp do teacher phụ trách
            var classes = await _context.Classes
                .Where(c => c.TeacherId == teacherId && c.IsActive)
                .ToListAsync();

            result.TotalClasses = classes.Count;

            // Chuẩn bị danh sách id lớp (int) — loại bỏ null nếu có
            var classIdsInt = classes
                .Where(c => c.ClassId != 0) // optional guard nếu id 0 không hợp lệ
                .Select(c => c.ClassId)
                .ToList();

            // 2️ Tổng số học sinh
            result.TotalStudents = await _context.Enrollments
                .Where(e => classIdsInt.Contains(e.ClassId) && e.RoleInClass == "Student")
                .CountAsync();

            var now = DateTime.UtcNow;

            // 3️ Số bài kiểm tra đang diễn ra
            result.ActiveExams = await _context.Exams
                .Where(e => e.ClassId != null && classIdsInt.Contains(e.ClassId.Value) && e.StartAt <= now && e.EndAt >= now)
                .CountAsync();

            // 4️⃣ Số bài cần chấm (ví dụ: đã nộp và cần chấm)
            result.PendingGrading = await _context.StudentExamAttempts
                .Include(a => a.Exam)
                .Where(a => a.Exam != null
                         && a.Status == "Submitted"
                         && a.Exam.ClassId != null
                         && classIdsInt.Contains(a.Exam.ClassId.Value))
                .CountAsync();

            // 5️ Học sinh có dấu hiệu gian lận (top 5)
            var highRisk = await _context.CheatingAlerts
                .Include(a => a.Attempt)
                    .ThenInclude(at => at.Exam)
                .Include(a => a.User)
                .Where(a => a.Attempt != null
                         && a.Attempt.Exam != null
                         && a.Attempt.Exam.ClassId != null
                         && classIdsInt.Contains(a.Attempt.Exam.ClassId.Value))
                .OrderByDescending(a => a.Severity)
                .Select(a => new HighRiskStudentDto
                {
                    StudentName = a.User.DisplayName,
                    ExamTitle = a.Attempt.Exam.Title,
                    FocusLossCount = a.Attempt.FocusLostCount,
                    CopyPasteCount = _context.FocusLogs.Count(f => f.AttemptId == a.AttemptId)
                })
                .Take(5)
                .ToListAsync();

            result.HighRiskStudents = highRisk;

            // 6️ Recent exams
            result.RecentExams = await _context.Exams
                .Include(e => e.Class)
                .Where(e => e.ClassId != null && classIdsInt.Contains(e.ClassId.Value))
                .OrderByDescending(e => e.StartAt)
                .Take(5)
                .Select(e => new RecentExamDto
                {
                    ExamId = e.ExamId,
                    Title = e.Title,
                    ClassName = e.Class != null ? e.Class.Title : string.Empty,
                    Status = e.EndAt < now ? "completed" :
                             e.StartAt > now ? "upcoming" : "ongoing"
                })
                .ToListAsync();

            // 1) student counts per class
            var studentCounts = await _context.Enrollments
                .Where(e => classIdsInt.Contains(e.ClassId) && e.RoleInClass == "Student")
                .GroupBy(e => e.ClassId)
                .Select(g => new { ClassId = g.Key, Count = g.Count() })
                .ToListAsync();

            // 2) exam counts per class
            var examCounts = await _context.Exams
                .Where(e => e.ClassId != null && classIdsInt.Contains(e.ClassId.Value))
                .GroupBy(e => e.ClassId.Value)
                .Select(g => new { ClassId = g.Key, Count = g.Count() })
                .ToListAsync();

            // 3) resource counts per class (tài liệu trong Resource table)
            var resourceCounts = await _context.Resources
                .Where(r => r.ClassId != null && classIdsInt.Contains(r.ClassId.Value) && !r.IsDeleted)
                .GroupBy(r => r.ClassId.Value)
                .Select(g => new { ClassId = g.Key, Count = g.Count() })
                .ToListAsync();

            // Map vào ClassStatDto
            var classStats = classes.Select(c =>
            {
                var cid = c.ClassId;
                var students = studentCounts.FirstOrDefault(x => x.ClassId == cid)?.Count ?? 0;
                var exams = examCounts.FirstOrDefault(x => x.ClassId == cid)?.Count ?? 0;
                var resources = resourceCounts.FirstOrDefault(x => x.ClassId == cid)?.Count ?? 0;

                return new ClassStatDto
                {
                    ClassId = cid,
                    ClassName = c.Title ?? string.Empty,
                    StudentCount = students,
                    ExamCount = exams,
                    ResourceCount = resources
                };
            }).ToList();

            result.ClassStats = classStats;

            return result;
        }

    }
}
