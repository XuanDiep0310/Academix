using Academix.Application.DTOs.Classes;
using Academix.Application.DTOs.Common;
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
    public class ClassService : IClassService
    {
        private readonly AcademixDbContext _context;
        private readonly ILogger<ClassService> _logger;

        public ClassService(AcademixDbContext context, ILogger<ClassService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<ClassListResponseDto>> GetAllClassesAsync(
            string? search = null,
            bool? isActive = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            try
            {
                var query = _context.Classes
                    .Include(c => c.CreatedByNavigation)
                    .Include(c => c.ClassMembers)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(c =>
                        c.ClassName.ToLower().Contains(searchLower) ||
                        c.ClassCode.ToLower().Contains(searchLower) ||
                        (c.Description != null && c.Description.ToLower().Contains(searchLower)));
                }

                if (isActive.HasValue)
                {
                    query = query.Where(c => c.IsActive == isActive.Value);
                }

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply sorting
                query = sortBy.ToLower() switch
                {
                    "classname" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(c => c.ClassName)
                        : query.OrderByDescending(c => c.ClassName),
                    "classcode" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(c => c.ClassCode)
                        : query.OrderByDescending(c => c.ClassCode),
                    _ => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(c => c.CreatedAt)
                        : query.OrderByDescending(c => c.CreatedAt)
                };

                // Apply pagination
                var classes = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new ClassResponseDto
                    {
                        ClassId = c.ClassId,
                        ClassName = c.ClassName,
                        ClassCode = c.ClassCode,
                        Description = c.Description,
                        CreatedBy = c.CreatedBy,
                        CreatedByName = c.CreatedByNavigation.FullName,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        TeacherCount = c.ClassMembers.Count(cm => cm.Role == UserRoles.Teacher),
                        StudentCount = c.ClassMembers.Count(cm => cm.Role == UserRoles.Student)
                    })
                    .ToListAsync();

                var response = new ClassListResponseDto
                {
                    Classes = classes,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return ApiResponse<ClassListResponseDto>.SuccessResponse(response, "Classes retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting classes: {ex.Message}");
                return ApiResponse<ClassListResponseDto>.ErrorResponse("Failed to retrieve classes");
            }
        }

        public async Task<ApiResponse<ClassDetailResponseDto>> GetClassByIdAsync(int classId)
        {
            try
            {
                var classEntity = await _context.Classes
                    .Include(c => c.CreatedByNavigation)
                    .Include(c => c.ClassMembers)
                        .ThenInclude(cm => cm.User)
                    .FirstOrDefaultAsync(c => c.ClassId == classId);

                if (classEntity == null)
                {
                    return ApiResponse<ClassDetailResponseDto>.ErrorResponse("Class not found");
                }

                var teachers = classEntity.ClassMembers
                    .Where(cm => cm.Role == UserRoles.Teacher)
                    .Select(cm => new ClassMemberDto
                    {
                        ClassMemberId = cm.ClassMemberId,
                        UserId = cm.UserId,
                        Email = cm.User.Email,
                        FullName = cm.User.FullName,
                        Role = cm.Role,
                        JoinedAt = cm.JoinedAt
                    })
                    .ToList();

                var students = classEntity.ClassMembers
                    .Where(cm => cm.Role == UserRoles.Student)
                    .Select(cm => new ClassMemberDto
                    {
                        ClassMemberId = cm.ClassMemberId,
                        UserId = cm.UserId,
                        Email = cm.User.Email,
                        FullName = cm.User.FullName,
                        Role = cm.Role,
                        JoinedAt = cm.JoinedAt
                    })
                    .ToList();

                var response = new ClassDetailResponseDto
                {
                    ClassId = classEntity.ClassId,
                    ClassName = classEntity.ClassName,
                    ClassCode = classEntity.ClassCode,
                    Description = classEntity.Description,
                    CreatedBy = classEntity.CreatedBy,
                    CreatedByName = classEntity.CreatedByNavigation.FullName,
                    IsActive = classEntity.IsActive,
                    CreatedAt = classEntity.CreatedAt,
                    UpdatedAt = classEntity.UpdatedAt,
                    Teachers = teachers,
                    Students = students,
                    TeacherCount = teachers.Count,
                    StudentCount = students.Count
                };

                return ApiResponse<ClassDetailResponseDto>.SuccessResponse(response, "Class retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting class {classId}: {ex.Message}");
                return ApiResponse<ClassDetailResponseDto>.ErrorResponse("Failed to retrieve class");
            }
        }

        public async Task<ApiResponse<ClassResponseDto>> CreateClassAsync(CreateClassRequestDto request, int createdBy)
        {
            try
            {
                // Check if class code exists
                if (await IsClassCodeExistsAsync(request.ClassCode))
                {
                    return ApiResponse<ClassResponseDto>.ErrorResponse("Class code already exists");
                }

                var classEntity = new Class
                {
                    ClassName = request.ClassName,
                    ClassCode = request.ClassCode.ToUpper(),
                    Description = request.Description,
                    CreatedBy = createdBy,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Classes.Add(classEntity);
                await _context.SaveChangesAsync();

                // Reload with creator info
                await _context.Entry(classEntity).Reference(c => c.CreatedByNavigation).LoadAsync();

                var response = new ClassResponseDto
                {
                    ClassId = classEntity.ClassId,
                    ClassName = classEntity.ClassName,
                    ClassCode = classEntity.ClassCode,
                    Description = classEntity.Description,
                    CreatedBy = classEntity.CreatedBy,
                    CreatedByName = classEntity.CreatedByNavigation.FullName,
                    IsActive = classEntity.IsActive,
                    CreatedAt = classEntity.CreatedAt,
                    UpdatedAt = classEntity.UpdatedAt,
                    TeacherCount = 0,
                    StudentCount = 0
                };

                _logger.LogInformation($"Class {classEntity.ClassCode} created successfully by user {createdBy}");
                return ApiResponse<ClassResponseDto>.SuccessResponse(response, "Class created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating class: {ex.Message}");
                return ApiResponse<ClassResponseDto>.ErrorResponse("Failed to create class");
            }
        }

        public async Task<ApiResponse<ClassResponseDto>> UpdateClassAsync(int classId, UpdateClassRequestDto request)
        {
            try
            {
                var classEntity = await _context.Classes
                    .Include(c => c.CreatedByNavigation)
                    .Include(c => c.ClassMembers)
                    .FirstOrDefaultAsync(c => c.ClassId == classId);

                if (classEntity == null)
                {
                    return ApiResponse<ClassResponseDto>.ErrorResponse("Class not found");
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(request.ClassName))
                {
                    classEntity.ClassName = request.ClassName;
                }

                if (request.Description != null)
                {
                    classEntity.Description = request.Description;
                }

                if (request.IsActive.HasValue)
                {
                    classEntity.IsActive = request.IsActive.Value;
                }

                classEntity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new ClassResponseDto
                {
                    ClassId = classEntity.ClassId,
                    ClassName = classEntity.ClassName,
                    ClassCode = classEntity.ClassCode,
                    Description = classEntity.Description,
                    CreatedBy = classEntity.CreatedBy,
                    CreatedByName = classEntity.CreatedByNavigation.FullName,
                    IsActive = classEntity.IsActive,
                    CreatedAt = classEntity.CreatedAt,
                    UpdatedAt = classEntity.UpdatedAt,
                    TeacherCount = classEntity.ClassMembers.Count(cm => cm.Role == UserRoles.Teacher),
                    StudentCount = classEntity.ClassMembers.Count(cm => cm.Role == UserRoles.Student)
                };

                _logger.LogInformation($"Class {classId} updated successfully");
                return ApiResponse<ClassResponseDto>.SuccessResponse(response, "Class updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating class {classId}: {ex.Message}");
                return ApiResponse<ClassResponseDto>.ErrorResponse("Failed to update class");
            }
        }

        public async Task<ApiResponse<string>> DeleteClassAsync(int classId)
        {
            try
            {
                var classEntity = await _context.Classes.FindAsync(classId);

                if (classEntity == null)
                {
                    return ApiResponse<string>.ErrorResponse("Class not found");
                }

                // Check if class can be deleted
                if (!await CanDeleteClassAsync(classId))
                {
                    return ApiResponse<string>.ErrorResponse("Cannot delete class with existing data. Consider deactivating instead.");
                }

                _context.Classes.Remove(classEntity);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Class {classId} deleted successfully");
                return ApiResponse<string>.SuccessResponse("Class deleted successfully", "Class deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting class {classId}: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to delete class");
            }
        }

        public async Task<ApiResponse<AddMembersResponseDto>> AddTeachersToClassAsync(int classId, AddMembersRequestDto request)
        {
            return await AddMembersToClassAsync(classId, request.UserIds, UserRoles.Teacher);
        }

        public async Task<ApiResponse<AddMembersResponseDto>> AddStudentsToClassAsync(int classId, AddMembersRequestDto request)
        {
            return await AddMembersToClassAsync(classId, request.UserIds, UserRoles.Student);
        }

        private async Task<ApiResponse<AddMembersResponseDto>> AddMembersToClassAsync(int classId, List<int> userIds, string role)
        {
            var response = new AddMembersResponseDto
            {
                TotalProcessed = userIds.Count
            };

            try
            {
                // Check if class exists
                var classExists = await _context.Classes.AnyAsync(c => c.ClassId == classId);
                if (!classExists)
                {
                    return ApiResponse<AddMembersResponseDto>.ErrorResponse("Class not found");
                }

                foreach (var userId in userIds)
                {
                    try
                    {
                        // Check if user exists and has correct role
                        var user = await _context.Users.FindAsync(userId);
                        if (user == null)
                        {
                            response.FailedMembers.Add(new FailedMemberDto
                            {
                                UserId = userId,
                                Reason = "User not found"
                            });
                            continue;
                        }

                        if (user.Role != role)
                        {
                            response.FailedMembers.Add(new FailedMemberDto
                            {
                                UserId = userId,
                                Email = user.Email,
                                FullName = user.FullName,
                                Reason = $"User is not a {role}"
                            });
                            continue;
                        }

                        if (!user.IsActive)
                        {
                            response.FailedMembers.Add(new FailedMemberDto
                            {
                                UserId = userId,
                                Email = user.Email,
                                FullName = user.FullName,
                                Reason = "User is inactive"
                            });
                            continue;
                        }

                        // Check if already in class
                        if (await IsUserInClassAsync(classId, userId))
                        {
                            response.FailedMembers.Add(new FailedMemberDto
                            {
                                UserId = userId,
                                Email = user.Email,
                                FullName = user.FullName,
                                Reason = "Already in class"
                            });
                            continue;
                        }

                        // Add to class
                        var classMember = new ClassMember
                        {
                            ClassId = classId,
                            UserId = userId,
                            Role = role,
                            JoinedAt = DateTime.UtcNow
                        };

                        _context.ClassMembers.Add(classMember);
                        await _context.SaveChangesAsync();

                        response.AddedMembers.Add(new ClassMemberDto
                        {
                            ClassMemberId = classMember.ClassMemberId,
                            UserId = user.UserId,
                            Email = user.Email,
                            FullName = user.FullName,
                            Role = role,
                            JoinedAt = classMember.JoinedAt
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error adding user {userId} to class {classId}: {ex.Message}");
                        response.FailedMembers.Add(new FailedMemberDto
                        {
                            UserId = userId,
                            Reason = "Internal error"
                        });
                    }
                }

                response.SuccessCount = response.AddedMembers.Count;
                response.FailedCount = response.FailedMembers.Count;

                _logger.LogInformation($"Added {response.SuccessCount} {role}(s) to class {classId}");
                return ApiResponse<AddMembersResponseDto>.SuccessResponse(
                    response,
                    $"Processed {response.TotalProcessed} users: {response.SuccessCount} added, {response.FailedCount} failed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding members to class {classId}: {ex.Message}");
                return ApiResponse<AddMembersResponseDto>.ErrorResponse("Failed to add members to class");
            }
        }

        public async Task<ApiResponse<string>> RemoveMemberFromClassAsync(int classId, int userId)
        {
            try
            {
                var classMember = await _context.ClassMembers
                    .FirstOrDefaultAsync(cm => cm.ClassId == classId && cm.UserId == userId);

                if (classMember == null)
                {
                    return ApiResponse<string>.ErrorResponse("Member not found in class");
                }

                _context.ClassMembers.Remove(classMember);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {userId} removed from class {classId}");
                return ApiResponse<string>.SuccessResponse("Member removed successfully", "Member removed from class successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing member from class: {ex.Message}");
                return ApiResponse<string>.ErrorResponse("Failed to remove member from class");
            }
        }

        public async Task<ApiResponse<List<ClassMemberDto>>> GetClassMembersAsync(int classId, string? role = null)
        {
            try
            {
                var query = _context.ClassMembers
                    .Include(cm => cm.User)
                    .Where(cm => cm.ClassId == classId);

                if (!string.IsNullOrEmpty(role))
                {
                    query = query.Where(cm => cm.Role == role);
                }

                var members = await query
                    .Select(cm => new ClassMemberDto
                    {
                        ClassMemberId = cm.ClassMemberId,
                        UserId = cm.UserId,
                        Email = cm.User.Email,
                        FullName = cm.User.FullName,
                        Role = cm.Role,
                        JoinedAt = cm.JoinedAt
                    })
                    .OrderBy(cm => cm.FullName)
                    .ToListAsync();

                return ApiResponse<List<ClassMemberDto>>.SuccessResponse(members, "Members retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting class members: {ex.Message}");
                return ApiResponse<List<ClassMemberDto>>.ErrorResponse("Failed to retrieve members");
            }
        }

        public async Task<ApiResponse<List<ClassMemberDto>>> GetClassStudentsAsync(int classId)
        {
            return await GetClassMembersAsync(classId, UserRoles.Student);
        }

        public async Task<ApiResponse<List<ClassMemberDto>>> GetClassTeachersAsync(int classId)
        {
            return await GetClassMembersAsync(classId, UserRoles.Teacher);
        }

        public async Task<ApiResponse<List<MyClassResponseDto>>> GetMyClassesAsync(int userId)
        {
            try
            {
                var myClasses = await _context.ClassMembers
                    .Include(cm => cm.Class)
                        .ThenInclude(c => c.ClassMembers)
                    .Where(cm => cm.UserId == userId)
                    .Select(cm => new MyClassResponseDto
                    {
                        ClassId = cm.Class.ClassId,
                        ClassName = cm.Class.ClassName,
                        ClassCode = cm.Class.ClassCode,
                        Description = cm.Class.Description,
                        MyRole = cm.Role,
                        JoinedAt = cm.JoinedAt,
                        IsActive = cm.Class.IsActive,
                        TeacherCount = cm.Class.ClassMembers.Count(m => m.Role == UserRoles.Teacher),
                        StudentCount = cm.Class.ClassMembers.Count(m => m.Role == UserRoles.Student)
                    })
                    .OrderByDescending(c => c.JoinedAt)
                    .ToListAsync();

                return ApiResponse<List<MyClassResponseDto>>.SuccessResponse(myClasses, "Classes retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting classes for user {userId}: {ex.Message}");
                return ApiResponse<List<MyClassResponseDto>>.ErrorResponse("Failed to retrieve classes");
            }
        }

        public async Task<ApiResponse<ClassStatisticsDto>> GetClassStatisticsAsync()
        {
            try
            {
                // OPTIMIZATION: Gộp 3 truy vấn đếm trên bảng Classes thành 1
                var classCountStats = await _context.Classes
                    .GroupBy(c => 1) // Gộp tất cả vào 1 nhóm
                    .Select(g => new
                    {
                        TotalClasses = g.Count(),
                        ActiveClasses = g.Count(c => c.IsActive),
                        InactiveClasses = g.Count(c => !c.IsActive)
                    })
                    .FirstOrDefaultAsync();

                var totalClasses = classCountStats?.TotalClasses ?? 0;
                var activeClasses = classCountStats?.ActiveClasses ?? 0;
                var inactiveClasses = classCountStats?.InactiveClasses ?? 0;

                // Các truy vấn này phức tạp hơn và riêng biệt, giữ nguyên
                var totalTeachers = await _context.ClassMembers
                    .Where(cm => cm.Role == UserRoles.Teacher)
                    .Select(cm => cm.UserId)
                    .Distinct()
                    .CountAsync();

                var totalStudents = await _context.ClassMembers
                    .Where(cm => cm.Role == UserRoles.Student)
                    .Select(cm => cm.UserId)
                    .Distinct()
                    .CountAsync();

                var avgStudentsPerClass = totalClasses > 0
                    ? await _context.ClassMembers
                        .Where(cm => cm.Role == UserRoles.Student)
                        .GroupBy(cm => cm.ClassId)
                        .Select(g => g.Count())
                        .AverageAsync()
                    : 0;

                // FIXED QUERY: Sửa lỗi "could not be translated"
                var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);

                // 1. Lấy dữ liệu thô từ CSDL
                var rawGrowthData = await _context.Classes
                    .Where(c => c.CreatedAt >= twelveMonthsAgo)
                    .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Year)   // Sắp xếp theo năm
                    .ThenBy(g => g.Month) // Rồi theo tháng
                    .ToListAsync();       // <-- Thực thi truy vấn, đưa về bộ nhớ

                // 2. Định dạng chuỗi trong bộ nhớ (C#)
                var classGrowth = rawGrowthData
                    .Select(g => new ClassGrowthDto
                    {
                        Month = $"{g.Year}-{g.Month:D2}", // Việc định dạng này giờ an toàn
                        Count = g.Count
                    })
                    .ToList();

                // Tạo đối tượng DTO kết quả
                var statistics = new ClassStatisticsDto
                {
                    TotalClasses = totalClasses,
                    ActiveClasses = activeClasses,
                    InactiveClasses = inactiveClasses,
                    TotalTeachers = totalTeachers,
                    TotalStudents = totalStudents,
                    AverageStudentsPerClass = Math.Round(avgStudentsPerClass, 2),
                    ClassGrowth = classGrowth
                };

                return ApiResponse<ClassStatisticsDto>.SuccessResponse(statistics, "Statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting class statistics: {ex.Message}");
                return ApiResponse<ClassStatisticsDto>.ErrorResponse("Failed to retrieve statistics");
            }
        }

        public async Task<bool> IsClassCodeExistsAsync(string classCode, int? excludeClassId = null)
        {
            var query = _context.Classes.Where(c => c.ClassCode == classCode.ToUpper());

            if (excludeClassId.HasValue)
            {
                query = query.Where(c => c.ClassId != excludeClassId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsUserInClassAsync(int classId, int userId)
        {
            return await _context.ClassMembers
                .AnyAsync(cm => cm.ClassId == classId && cm.UserId == userId);
        }

        public async Task<bool> CanDeleteClassAsync(int classId)
        {
            // Check if class has any related data (materials, exams, etc.)
            // For now, just check if class exists
            var classEntity = await _context.Classes.FindAsync(classId);
            return classEntity != null;
        }
    }
}
