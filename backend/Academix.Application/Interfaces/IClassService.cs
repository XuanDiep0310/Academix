using Academix.Application.DTOs.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IClassService
    {
        Task<ClassDto> GetClassByIdAsync(int classId);
        Task<IEnumerable<ClassDto>> GetAllClassesAsync();
        Task<ClassDto> CreateClassAsync(CreateClassRequest request);
        Task<bool> UpdateClassAsync(int classId, UpdateClassRequest request);
        Task<bool> DeleteClassAsync(int classId);
        Task<IEnumerable<MyClassResponse>> GetMyClassesAsync(int userId);
    }
}
