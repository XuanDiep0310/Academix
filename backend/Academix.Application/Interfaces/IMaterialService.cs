using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IMaterialService
    {
        // Material CRUD Operations
        Task<ApiResponse<MaterialListResponseDto>> GetMaterialsByClassAsync(
            int classId,
            string? type = null,
            string? search = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc");

        Task<ApiResponse<MaterialResponseDto>> GetMaterialByIdAsync(int materialId);

        Task<ApiResponse<MaterialResponseDto>> CreateMaterialAsync(int classId, CreateMaterialRequestDto request, int uploadedBy);

        Task<ApiResponse<MaterialResponseDto>> UpdateMaterialAsync(int materialId, UpdateMaterialRequestDto request);

        Task<ApiResponse<string>> DeleteMaterialAsync(int materialId);

        // Statistics
        Task<ApiResponse<MaterialStatisticsDto>> GetMaterialStatisticsAsync(int? classId = null);

        // Validation
        Task<bool> CanAccessMaterialAsync(int materialId, int userId, string userRole);
    }
}
