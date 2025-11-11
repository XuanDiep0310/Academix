using Academix.Application.DTOs.Classes;
using Academix.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IClassService
    {
        // Class CRUD Operations
        Task<ApiResponse<ClassListResponseDto>> GetAllClassesAsync(
            string? search = null,
            bool? isActive = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc");

        Task<ApiResponse<ClassDetailResponseDto>> GetClassByIdAsync(int classId);

        Task<ApiResponse<ClassResponseDto>> CreateClassAsync(CreateClassRequestDto request, int createdBy);

        Task<ApiResponse<ClassResponseDto>> UpdateClassAsync(int classId, UpdateClassRequestDto request);

        Task<ApiResponse<string>> DeleteClassAsync(int classId);

        // Class Member Management
        Task<ApiResponse<AddMembersResponseDto>> AddTeachersToClassAsync(int classId, AddMembersRequestDto request);

        Task<ApiResponse<AddMembersResponseDto>> AddStudentsToClassAsync(int classId, AddMembersRequestDto request);

        Task<ApiResponse<string>> RemoveMemberFromClassAsync(int classId, int userId);

        Task<ApiResponse<List<ClassMemberDto>>> GetClassMembersAsync(int classId, string? role = null);

        Task<ApiResponse<List<ClassMemberDto>>> GetClassStudentsAsync(int classId);

        Task<ApiResponse<List<ClassMemberDto>>> GetClassTeachersAsync(int classId);

        // My Classes
        Task<ApiResponse<List<MyClassResponseDto>>> GetMyClassesAsync(int userId);

        // Statistics
        Task<ApiResponse<ClassStatisticsDto>> GetClassStatisticsAsync();

        // Validation
        Task<bool> IsClassCodeExistsAsync(string classCode, int? excludeClassId = null);
        Task<bool> IsUserInClassAsync(int classId, int userId);
        Task<bool> CanDeleteClassAsync(int classId);
    }
}
