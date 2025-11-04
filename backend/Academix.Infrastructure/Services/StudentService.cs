using Academix.Application.DTOs.Student;
using Academix.Application.Interfaces;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Academix.Infrastructure.Services
{
    public class StudentService : IStudentService
    {
        private readonly AcademixDbContext _context;

        public StudentService(AcademixDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentResponse>> GetStudentsAsync(int? classId, string? query)
        {
            var queryBase = _context.Enrollments
                .Include(e => e.Class)
                .Include(e => e.User)
                .AsQueryable();

            if (classId.HasValue)
                queryBase = queryBase.Where(e => e.ClassId == classId.Value);

            if (!string.IsNullOrEmpty(query))
                queryBase = queryBase.Where(e =>
                    e.User.DisplayName.Contains(query) ||
                    e.User.Email.Contains(query));

            var result = await queryBase
                .Select(e => new StudentResponse
                {
                    Id = e.UserId,
                    Name = e.User.DisplayName,
                    Email = e.User.Email,
                    ClassName = e.Class.Title,

                    // Subquery tính trung bình và số bài thi
                    AvgScore = _context.StudentExamAttempts
                        .Where(a => a.UserId == e.UserId)
                        .Average(a => (double?)a.Score),

                    ExamCount = _context.StudentExamAttempts
                        .Count(a => a.UserId == e.UserId)
                })
                .ToListAsync();

            return result;
        }






        // Thêm sinh viên vào lớp học (tạo Enrollment)
        public async Task<StudentResponse> AddStudentToClassAsync(CreateStudentRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                // 🟢 Tạo tên mặc định từ email (vd: "ngan.luong@gmail.com" → "ngan.luong")
                var defaultName = request.Email.Split('@')[0];

                user = new User
                {
                    DisplayName = defaultName,
                    Email = request.Email,
                    NormalizedEmail = request.Email.ToUpper(),
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            bool existed = await _context.Enrollments
                .AnyAsync(e => e.UserId == user.UserId && e.ClassId == request.ClassId);

            if (existed)
                throw new InvalidOperationException("Học sinh đã có trong lớp này.");

            var enrollment = new Enrollment
            {
                UserId = user.UserId,
                ClassId = request.ClassId,
                RoleInClass = "Student",
                JoinedAt = DateTime.UtcNow,
                IsApproved = true,
                IsActive = true
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var classEntity = await _context.Classes.FindAsync(request.ClassId);

            return new StudentResponse
            {
                Id = user.UserId,
                Name = user.DisplayName,
                Email = user.Email,
                ClassName = classEntity?.Title ?? "",
                AvgScore = null,
                ExamCount = 0
            };
        }


        // Gỡ sinh viên khỏi lớp học
        public async Task<bool> RemoveStudentFromClassAsync(int classId, int studentId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == studentId && e.ClassId == classId);

            if (enrollment == null)
                return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<StudentStatsResponse> GetStudentStatsAsync(int studentId)
        {
            var attempts = await _context.StudentExamAttempts
                .Where(a => a.UserId == studentId)
                .ToListAsync();

            if (!attempts.Any())
                return new StudentStatsResponse
                {
                    StudentId = studentId,
                    ExamCount = 0,
                    AverageScore = null
                };

            return new StudentStatsResponse
            {
                StudentId = studentId,
                ExamCount = attempts.Count,
                AverageScore = (double?)attempts.Average(a => a.Score)
            };
        }

        public async Task<byte[]> ExportStudentsAsync(int? classId)
        {
            var students = await GetStudentsAsync(classId, null);

            var csv = new StringBuilder();
            csv.AppendLine("Id,Name,Email,ClassName,AvgScore,ExamCount");

            foreach (var s in students)
                csv.AppendLine($"{s.Id},{s.Name},{s.Email},{s.ClassName},{s.AvgScore},{s.ExamCount}");

            var utf8WithBom = new UTF8Encoding(true);
            return utf8WithBom.GetBytes(csv.ToString());
        }

        public async Task<IEnumerable<StudentResponse>> AddStudentsToClassAsync(CreateStudentsRequest request)
        {
            var responses = new List<StudentResponse>();

            foreach (var student in request.Students)
            {
                var email = student.Email.Trim();
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    var defaultName = email.Split('@')[0];

                    user = new User
                    {
                        DisplayName = defaultName,
                        Email = email,
                        NormalizedEmail = email.ToUpper(),
                        IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                bool existed = await _context.Enrollments
                    .AnyAsync(e => e.UserId == user.UserId && e.ClassId == request.ClassId);

                if (existed)
                    continue;

                var enrollment = new Enrollment
                {
                    UserId = user.UserId,
                    ClassId = request.ClassId,
                    RoleInClass = "Student",
                    JoinedAt = DateTime.UtcNow,
                    IsApproved = true,
                    IsActive = true
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                var classEntity = await _context.Classes.FindAsync(request.ClassId);

                responses.Add(new StudentResponse
                {
                    Id = user.UserId,
                    Name = user.DisplayName,
                    Email = user.Email,
                    ClassName = classEntity?.Title ?? "",
                    AvgScore = null,
                    ExamCount = 0
                });
            }

            return responses;
        }


    }
}
