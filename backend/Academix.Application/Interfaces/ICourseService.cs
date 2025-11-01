using Academix.Application.DTOs.Class;
using Academix.Application.DTOs.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto> GetCourseByIdAsync(int courseId);
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> CreateCoursesAsync(CreateCourseRequest request);
        Task<bool> UpdateCourseAsync(int courseId, UpdateCourseRequest request);
        Task<bool> DeleteCourseAsync(int courseId);
    }
}
