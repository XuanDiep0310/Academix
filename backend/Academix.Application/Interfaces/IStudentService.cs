using Academix.Application.DTOs.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponse>> GetStudentsAsync(int? classId, string? query);
        Task<StudentResponse> AddStudentToClassAsync(CreateStudentRequest request);
        Task<bool> RemoveStudentFromClassAsync(int classId, int studentId);
        Task<StudentStatsResponse> GetStudentStatsAsync(int studentId);
        Task<byte[]> ExportStudentsAsync(int? classId);
        Task<IEnumerable<StudentResponse>> AddStudentsToClassAsync(CreateStudentsRequest request);

    }
}
