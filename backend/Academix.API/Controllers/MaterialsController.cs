using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Materials;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Academix.API.Controllers
{
    [ApiController]
    [Route("api/classes/{classId}/materials")]
    [Authorize]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<MaterialsController> _logger;

        public MaterialsController(
            IMaterialService materialService,
            IFileStorageService fileStorageService,
            ILogger<MaterialsController> logger)
        {
            _materialService = materialService;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        /// <summary>
        /// Get all materials in a class (Teachers and Students)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.Student}")]
        [ProducesResponseType(typeof(ApiResponse<MaterialListResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMaterialsByClass(
            int classId,
            [FromQuery] string? type = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortOrder = "desc")
        {
            var result = await _materialService.GetMaterialsByClassAsync(classId, type, search, page, pageSize, sortBy, sortOrder);
            return Ok(result);
        }

        /// <summary>
        /// Get material by ID (Teachers and Students)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.Student}")]
        [ProducesResponseType(typeof(ApiResponse<MaterialResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MaterialResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMaterialById(int classId, int id)
        {
            var result = await _materialService.GetMaterialByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            // Verify material belongs to the class
            if (result.Data!.ClassId != classId)
            {
                return BadRequest(ApiResponse<MaterialResponseDto>.ErrorResponse("Material does not belong to this class"));
            }

            return Ok(result);
        }

        /// <summary>
        /// Create material with link (Teachers only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = UserRoles.Teacher)]
        [ProducesResponseType(typeof(ApiResponse<MaterialResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<MaterialResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMaterial(int classId, [FromBody] CreateMaterialRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<MaterialResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<MaterialResponseDto>.ErrorResponse("Invalid user"));
            }

            var result = await _materialService.CreateMaterialAsync(classId, request, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetMaterialById), new { classId, id = result.Data!.MaterialId }, result);
        }

        /// <summary>
        /// Upload material file (Teachers only)
        /// </summary>
        [HttpPost("upload")]
        [Authorize(Roles = UserRoles.Teacher)]
        [ProducesResponseType(typeof(ApiResponse<MaterialResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<MaterialResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadMaterial(int classId, [FromForm] UploadMaterialRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<MaterialResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<MaterialResponseDto>.ErrorResponse("Invalid user"));
            }

            // Upload file
            var uploadResult = await _fileStorageService.UploadFileAsync(request.File, $"classes/{classId}");

            if (!uploadResult.Success)
            {
                return BadRequest(ApiResponse<MaterialResponseDto>.ErrorResponse(uploadResult.ErrorMessage ?? "Failed to upload file"));
            }

            // Determine material type from file extension
            var extension = _fileStorageService.GetFileExtension(request.File.FileName);
            var materialType = _fileStorageService.DetermineMaterialType(extension);

            // Create material record
            var createRequest = new CreateMaterialRequestDto
            {
                Title = request.Title,
                Description = request.Description,
                MaterialType = materialType,
                FileUrl = uploadResult.FileUrl,
                FileName = uploadResult.FileName,
                FileSize = uploadResult.FileSize
            };

            var result = await _materialService.CreateMaterialAsync(classId, createRequest, userId);

            if (!result.Success)
            {
                // Delete uploaded file if material creation fails
                await _fileStorageService.DeleteFileAsync(uploadResult.FileUrl!);
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetMaterialById), new { classId, id = result.Data!.MaterialId }, result);
        }

        /// <summary>
        /// Update material (Teachers only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Teacher)]
        [ProducesResponseType(typeof(ApiResponse<MaterialResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<MaterialResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<MaterialResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMaterial(int classId, int id, [FromBody] UpdateMaterialRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<MaterialResponseDto>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                ));
            }

            var result = await _materialService.UpdateMaterialAsync(id, request);

            if (!result.Success)
            {
                return result.Message == "Material not found" ? NotFound(result) : BadRequest(result);
            }

            // Verify material belongs to the class
            if (result.Data!.ClassId != classId)
            {
                return BadRequest(ApiResponse<MaterialResponseDto>.ErrorResponse("Material does not belong to this class"));
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete material (Teachers only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Teacher)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMaterial(int classId, int id)
        {
            var result = await _materialService.DeleteMaterialAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Download material file (Teachers and Students)
        /// </summary>
        [HttpGet("{id}/download")]
        [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.Student}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadMaterial(int classId, int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("Invalid user"));
            }

            // Check access permission
            var canAccess = await _materialService.CanAccessMaterialAsync(id, userId, userRole!);
            if (!canAccess)
            {
                return Forbid();
            }

            var materialResult = await _materialService.GetMaterialByIdAsync(id);
            if (!materialResult.Success || materialResult.Data == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Material not found"));
            }

            var material = materialResult.Data;

            if (string.IsNullOrEmpty(material.FileUrl))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("No file available for download"));
            }

            // Convert URL to physical path
            var relativePath = material.FileUrl.TrimStart('/');
            var filePath = Path.Combine("wwwroot", relativePath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(ApiResponse<string>.ErrorResponse("File not found on server"));
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(material.FileUrl);

            return File(fileBytes, contentType, material.FileName ?? "download");
        }

        /// <summary>
        /// Get material statistics for a class (Teachers only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = UserRoles.Teacher)]
        [ProducesResponseType(typeof(ApiResponse<MaterialStatisticsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMaterialStatistics(int classId)
        {
            var result = await _materialService.GetMaterialStatisticsAsync(classId);
            return Ok(result);
        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                ".avi" => "video/x-msvideo",
                ".mov" => "video/quicktime",
                _ => "application/octet-stream"
            };
        }
    }
}
