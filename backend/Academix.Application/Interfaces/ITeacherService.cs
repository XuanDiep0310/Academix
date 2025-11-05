using Academix.Application.DTOs.Class;
using Academix.Application.DTOs.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface ITeacherService
    {
        Task<TeacherDashboardDto> GetDashboardAsync(int teacherId);
        Task<IEnumerable<MyClassResponse>> GetTeachingClassesAsync(int teacherId);
        Task<IEnumerable<ExamDto>> GetExamsAsync(int teacherId);
        Task<int> CreateExamAsync(int teacherId, CreateExamRequest request);
    }
}
