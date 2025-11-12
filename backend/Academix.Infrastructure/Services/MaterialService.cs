using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Materials;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly AcademixDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<MaterialService> _logger;

        public MaterialService(
            AcademixDbContext context,
            IFileStorageService fileStorageService,
            ILogger<MaterialService> logger)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<ApiResponse<MaterialListResponseDto>> GetMaterialsByClassAsync(
            int classId,
            string? type = null,
            string? search = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            try
            {
                var query = _context.Materials
                    .Include(m => m.UploadedByNavigation)
                    .Include(m => m.Class)
                    .Where(m => m.ClassId == classId)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(m => m.MaterialType == type);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(m =>
                        m.Title.ToLower().Contains(searchLower) ||
                        (m.Description != null && m.Description.ToLower().Contains(searchLower)) ||
                        (m.FileName != null && m.FileName.ToLower().Contains(searchLower)));
                }

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply sorting
                query = sortBy.ToLower() switch
                {
                    "title" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(m => m.Title)
                        : query.OrderByDescending(m => m.Title),
                    "type" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(m => m.MaterialType)
                        : query.OrderByDescending(m => m.MaterialType),
                    "size" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(m => m.FileSize)
                        : query.OrderByDescending(m => m.FileSize),
                    _ => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(m => m.CreatedAt)
                        : query.OrderByDescending(m => m.CreatedAt)
                };

                // Apply pagination
                var materials = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new MaterialResponseDto
                    {
                        MaterialId = m.MaterialId,
                        ClassId = m.ClassId,
                        ClassName = m.Class.ClassName,
                        Title = m.Title,
                        Description = m.Description,
                        MaterialType = m.MaterialType,
                        FileUrl = m.FileUrl,
                        FileName = m.FileName,
                        FileSize = m.FileSize,
                        FileSizeFormatted = m.FileSize.HasValue
                            ? _fileStorageService.FormatFileSize(m.FileSize.Value)
                            : null,
                        UploadedBy = m.UploadedBy,
                        UploadedByName = m.UploadedByNavigation.FullName,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt
                    })
                    .ToListAsync();

                var response = new MaterialListResponseDto
                {
                    Materials = materials,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return ApiResponse<MaterialListResponseDto>.SuccessResponse(response, "Materials retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting materials for class {classId}: {ex.Message}");
                return ApiResponse<MaterialListResponseDto>.ErrorResponse("Failed to retrieve materials");
            }
        }

        public async Task<ApiResponse<MaterialResponseDto>> GetMaterialByIdAsync(int materialId)
        {
            try
            {
                var material = await _context.Materials
                    .Include(m => m.UploadedByNavigation)
                    .Include(m => m.Class)
                    .FirstOrDefaultAsync(m => m.MaterialId == materialId);

                if (material == null)
                {
                    return ApiResponse<MaterialResponseDto>.ErrorResponse("Material not found");
                }

                var response = new MaterialResponseDto
                {
                    MaterialId = material.MaterialId,
                    ClassId = material.ClassId,
                    ClassName = material.Class.ClassName,
                    Title = material.Title,
                    Description = material.Description,
                    MaterialType = material.MaterialType,
                    FileUrl = material.FileUrl,
                    FileName = material.FileName,
                    FileSize = material.FileSize,
                    FileSizeFormatted = material.FileSize.HasValue
                        ? _fileStorageService.FormatFileSize(material.FileSize.Value)
                        : null,
                    UploadedBy = material.UploadedBy,
                    UploadedByName = material.UploadedByNavigation.FullName,
                    CreatedAt = material.CreatedAt,
                    UpdatedAt = material.UpdatedAt
                };

                return ApiResponse<MaterialResponseDto>.SuccessResponse(response, "Material retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting material {materialId}: {ex.Message}");
                return ApiResponse<MaterialResponseDto>.ErrorResponse("Failed to retrieve material");
            }
        }

        public async Task<ApiResponse<MaterialResponseDto>> CreateMaterialAsync(
            int classId,
            CreateMaterialRequestDto request,
            int uploadedBy)
        {
            try
            {
                // Check if class exists
                var classExists = await _context.Classes.AnyAsync(c => c.ClassId == classId);
                if (!classExists)
                {
                    return ApiResponse<MaterialResponseDto>.ErrorResponse("Class not found");
                }

                // Validate material type
                if (!MaterialTypes.IsValid(request.MaterialType))
                {
                    return ApiResponse<MaterialResponseDto>.ErrorResponse("Invalid material type");
                }

                var material = new Material
                {
                    ClassId = classId,
                    Title = request.Title,
                    Description = request.Description,
                    MaterialType = request.MaterialType,
                    FileUrl = request.FileUrl,
                    FileName = request.FileName,
                    FileSize = request.FileSize,
                    UploadedBy = uploadedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Materials.Add(material);
                await _context.SaveChangesAsync();

                // Reload with relationships
                await _context.Entry(material).Reference(m => m.UploadedByNavigation).LoadAsync();
                await _context.Entry(material).Reference(m => m.Class).LoadAsync();

                var response = new MaterialResponseDto
                {
                    MaterialId = material.MaterialId,
                    ClassId = material.ClassId,
                    ClassName = material.Class.ClassName,
                    Title = material.Title,
                    Description = material.Description,
                    MaterialType = material.MaterialType,
                    FileUrl = material.FileUrl,
                    FileName = material.FileName,
                    FileSize = material.FileSize,
                    FileSizeFormatted = material.FileSize.HasValue
                        ? _fileStorageService.FormatFileSize(material.FileSize.Value)
                        : null,
                    UploadedBy = material.UploadedBy,
                    UploadedByName = material.UploadedByNavigation.FullName,
                    CreatedAt = material.CreatedAt,
                    UpdatedAt = material.UpdatedAt
                };

                _logger.LogInformation($"Material {material.MaterialId} created successfully in class {classId}");
                return ApiResponse<MaterialResponseDto>.SuccessResponse(response, "Material created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating material: {ex.Message}");
                return ApiResponse<MaterialResponseDto>.ErrorResponse("Failed to create material");
            }
        }

        public async Task<ApiResponse<MaterialResponseDto>> UpdateMaterialAsync(int materialId, UpdateMaterialRequestDto request)
        {
            try
            {
                var material = await _context.Materials
                    .Include(m => m.UploadedByNavigation)
                    .Include(m => m.Class)
                    .FirstOrDefaultAsync(m => m.MaterialId == materialId);

                if (material == null)
                {
                    return ApiResponse<MaterialResponseDto>.ErrorResponse("Material not found");
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(request.Title))
                {
                    material.Title = request.Title;
                }

                if (request.Description != null)
                {
                    material.Description = request.Description;
                }

                if (!string.IsNullOrEmpty(request.FileUrl))
                {
                    material.FileUrl = request.FileUrl;
                }

                material.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new MaterialResponseDto
                {
                    MaterialId = material.MaterialId,
                    ClassId = material.ClassId,
                    ClassName = material.Class.ClassName,
                    Title = material.Title,
                    Description = material.Description,
                    MaterialType = material.MaterialType,
                    FileUrl = material.FileUrl,
                    FileName = material.FileName,
                    FileSize = material.FileSize,
                    FileSizeFormatted = material.FileSize.HasValue
                        ? _fileStorageService.FormatFileSize(material.FileSize.Value)
                        : null,
                    UploadedBy = material.UploadedBy,
                    UploadedByName = material.UploadedByNavigation.FullName,
                    CreatedAt = material.CreatedAt,
                    UpdatedAt = material.UpdatedAt
                };

                _logger.LogInformation($"Material {materialId} updated successfully");
                return ApiResponse<MaterialResponseDto>.SuccessResponse(response, "Material updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating material {materialId}: {ex.Message}");
                return ApiResponse<MaterialResponseDto>.ErrorResponse("Failed to update material");
            }
        }

        public async Task<ApiResponse<string>> DeleteMaterialAsync(int materialId)
        {
            try
            {
                var material = await _context.Materials.FindAsync(materialId);

                if (material == null)
                {
                    return ApiResponse<string>.ErrorResponse("Material not found");
                }

                // Delete file from storage if exists
                if (!string.IsNullOrEmpty(material.FileUrl))
                {
                    await _fileStorageService.DeleteFileAsync(material.FileUrl);
                }

                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Material {materialId} deleted successfully");
                return ApiResponse<string>.SuccessResponse("Material deleted successfully", "Material deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting material {materialId}: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to delete material");
            }
        }

        public async Task<ApiResponse<MaterialStatisticsDto>> GetMaterialStatisticsAsync(int? classId = null)
        {
            try
            {
                var query = _context.Materials.AsQueryable();

                if (classId.HasValue)
                {
                    query = query.Where(m => m.ClassId == classId.Value);
                }

                var totalMaterials = await query.CountAsync();

                var materialsByType = await query
                    .GroupBy(m => m.MaterialType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Type, x => x.Count);

                var totalStorageUsed = await query
                    .Where(m => m.FileSize.HasValue)
                    .SumAsync(m => m.FileSize.Value);

                var today = DateTime.UtcNow.Date;
                var weekAgo = DateTime.UtcNow.AddDays(-7);
                var monthAgo = DateTime.UtcNow.AddMonths(-1);

                var materialsUploadedToday = await query.CountAsync(m => m.CreatedAt.Date == today);
                var materialsUploadedThisWeek = await query.CountAsync(m => m.CreatedAt >= weekAgo);
                var materialsUploadedThisMonth = await query.CountAsync(m => m.CreatedAt >= monthAgo);

                var topUploaders = await query
                    .GroupBy(m => new { m.UploadedBy, m.UploadedByNavigation.FullName, m.UploadedByNavigation.Email })
                    .Select(g => new TopUploaderDto
                    {
                        UserId = g.Key.UploadedBy,
                        FullName = g.Key.FullName,
                        Email = g.Key.Email,
                        MaterialCount = g.Count()
                    })
                    .OrderByDescending(u => u.MaterialCount)
                    .Take(10)
                    .ToListAsync();

                var statistics = new MaterialStatisticsDto
                {
                    TotalMaterials = totalMaterials,
                    MaterialsByType = materialsByType,
                    TotalStorageUsed = totalStorageUsed,
                    TotalStorageUsedFormatted = _fileStorageService.FormatFileSize(totalStorageUsed),
                    MaterialsUploadedToday = materialsUploadedToday,
                    MaterialsUploadedThisWeek = materialsUploadedThisWeek,
                    MaterialsUploadedThisMonth = materialsUploadedThisMonth,
                    TopUploaders = topUploaders
                };

                return ApiResponse<MaterialStatisticsDto>.SuccessResponse(statistics, "Statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting material statistics: {ex.Message}");
                return ApiResponse<MaterialStatisticsDto>.ErrorResponse("Failed to retrieve statistics");
            }
        }

        public async Task<bool> CanAccessMaterialAsync(int materialId, int userId, string userRole)
        {
            try
            {
                // Admin can access all materials
                if (userRole == UserRoles.Admin)
                {
                    return true;
                }

                var material = await _context.Materials.FindAsync(materialId);
                if (material == null)
                {
                    return false;
                }

                // Check if user is a member of the class
                var isMember = await _context.ClassMembers
                    .AnyAsync(cm => cm.ClassId == material.ClassId && cm.UserId == userId);

                return isMember;
            }
            catch
            {
                return false;
            }
        }
    }
}
