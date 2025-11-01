using Academix.Application.DTOs.Course;
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
    public class CourseService : ICourseService
    {
        private readonly AcademixDbContext _context;

        public CourseService(AcademixDbContext context)
        {
            _context = context;
        }

        private static CourseDto MapToDto(Course entity)
        {
            return new CourseDto
            {
                CourseId = entity.CourseId,
                OrganizationId = entity.OrganizationId,
                Code = entity.Code,
                Title = entity.Title,
                ShortDescription = entity.ShortDescription,
                LongDescription = entity.LongDescription,
                CreatedBy = entity.CreatedBy,
                IsPublished = entity.IsPublished,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<CourseDto> CreateCoursesAsync(CreateCourseRequest request)
        {
            var newCourse = new Course
            {
                OrganizationId = request.OrganizationId,
                Code = request.Code,
                Title = request.Title,
                ShortDescription = request.ShortDescription,
                LongDescription = request.LongDescription,
                CreatedBy = request.CreatedBy,
                IsPublished = request.IsPublished,
                CreatedAt = request.CreatedAt == default ? DateTime.UtcNow : request.CreatedAt
            };

            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            return MapToDto(newCourse);
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _context.Courses.ToListAsync();
            return courses.Select(MapToDto);
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int courseId)
        {
            var entity = await _context.Courses.FindAsync(courseId);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<bool> UpdateCourseAsync(int courseId, UpdateCourseRequest request)
        {
            var existingCourse = await _context.Courses.FindAsync(courseId);
            if (existingCourse == null)
                return false;

            existingCourse.OrganizationId = request.OrganizationId;
            existingCourse.Code = request.Code;
            existingCourse.Title = request.Title;
            existingCourse.ShortDescription = request.ShortDescription;
            existingCourse.LongDescription = request.LongDescription;
            existingCourse.IsPublished = request.IsPublished;

            _context.Courses.Update(existingCourse);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var existingCourse = await _context.Courses.FindAsync(courseId);
            if (existingCourse == null)
                return false;

            _context.Courses.Remove(existingCourse);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
