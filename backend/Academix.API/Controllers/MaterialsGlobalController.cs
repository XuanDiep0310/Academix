using Academix.Application.DTOs.Common;
using Academix.Application.DTOs.Materials;
using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Academix.API.Controllers
{
    [ApiController]
    [Route("api/materials")]
    [Authorize]
    public class MaterialsGlobalController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialsGlobalController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        /// <summary>
        /// Get global material statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(typeof(ApiResponse<MaterialStatisticsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGlobalStatistics()
        {
            var result = await _materialService.GetMaterialStatisticsAsync();
            return Ok(result);
        }
    }
}
