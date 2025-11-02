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
            var studentsQuery = _context.Enrollments
                .Include(e => e.Class)
                .Include(e => e.User)
                .AsQueryable();

            if (classId.HasValue)
                studentsQuery = studentsQuery.Where(e => e.ClassId == classId.Value);

            if (!string.IsNullOrEmpty(query))
                studentsQuery = studentsQuery.Where(e =>
                    e.User.DisplayName.Contains(query) ||
                    e.User.Email.Contains(query));

            var result = await studentsQuery
                .Select(e => new StudentResponse
                {
                    Id = e.UserId,
                    Name = e.User.DisplayName,
                    Email = e.User.Email,
                    ClassName = e.Class.Title,
                    AvgScore = null,
                    ExamCount = 0
                })
                .ToListAsync();

            return result;
        }

        public async Task<StudentResponse> AddStudentAsync(CreateStudentRequest request)
        {
            // Kiểm tra User đã tồn tại theo email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                user = new User
                {
                    DisplayName = request.Name,
                    Email = request.Email,
                    NormalizedEmail = request.Email.ToUpper(),
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra xem học sinh đã có trong lớp chưa
            var existed = await _context.Enrollments
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

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == studentId);

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
            {
                csv.AppendLine($"{s.Id},{s.Name},{s.Email},{s.ClassName},{s.AvgScore},{s.ExamCount}");
            }

            // Ghi BOM đầu file để Excel nhận đúng UTF-8
            var utf8WithBom = new UTF8Encoding(true);
            return utf8WithBom.GetBytes(csv.ToString());
        }

    }
}
