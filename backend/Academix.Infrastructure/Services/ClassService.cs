using Academix.Application.DTOs.Class;
using Academix.Application.Interfaces;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class ClassService : IClassService
    {
        private readonly AcademixDbContext _context;

        public ClassService(AcademixDbContext context)
        {
            _context = context;
        }
        private static ClassDto MapToDto(Class entity)
        {
            return new ClassDto
            {
                ClassId = entity.ClassId,
                CourseId = entity.CourseId,
                OrganizationId = entity.OrganizationId,
                Title = entity.Title,
                TeacherId = entity.TeacherId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                EnrollmentCode = entity.EnrollmentCode,
                MaxStudents = entity.MaxStudents,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<ClassDto> CreateClassAsync(CreateClassRequest request)
        {
            var newClass = new Class
            {
                CourseId = request.CourseId,
                OrganizationId = request.OrganizationId,
                Title = request.Title,
                TeacherId = request.TeacherId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                EnrollmentCode = request.EnrollmentCode,
                MaxStudents = request.MaxStudents,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return MapToDto(newClass);
        }

        public async Task<IEnumerable<ClassDto>> GetAllClassesAsync()
        {
            var classes = await _context.Classes.ToListAsync();
            return classes.Select(MapToDto);
        }

        public async Task<ClassDto> GetClassByIdAsync(int classId)
        {
            var entity = await _context.Classes.FindAsync(classId);
            return entity == null ? null! : MapToDto(entity);
        }

        public async Task<bool> UpdateClassAsync(int classId, UpdateClassRequest request)
        {
            var existingClass = await _context.Classes.FindAsync(classId);
            if (existingClass == null)
                return false;

            existingClass.CourseId = request.CourseId;
            existingClass.OrganizationId = request.OrganizationId;
            existingClass.Title = request.Title;
            existingClass.TeacherId = request.TeacherId;
            existingClass.StartDate = request.StartDate;
            existingClass.EndDate = request.EndDate;
            existingClass.EnrollmentCode = request.EnrollmentCode;
            existingClass.MaxStudents = request.MaxStudents;
            existingClass.IsActive = request.IsActive;

            _context.Classes.Update(existingClass);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteClassAsync(int classId)
        {
            var existingClass = await _context.Classes.FindAsync(classId);
            if (existingClass == null)
                return false;

            _context.Classes.Remove(existingClass);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MyClassResponse>> GetMyClassesAsync(int userId)
        {
            return await _context.Enrollments
                .Where(e => e.UserId == userId && e.IsActive)
                .Include(e => e.Class)
                .Select(e => new MyClassResponse
                {
                    ClassId = e.Class.ClassId,
                    Title = e.Class.Title,
                    CourseId = e.Class.CourseId
                })
                .ToListAsync();
        }
    }
}
