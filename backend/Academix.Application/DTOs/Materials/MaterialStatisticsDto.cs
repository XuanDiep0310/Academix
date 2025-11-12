using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Materials
{
    public class MaterialStatisticsDto
    {
        public int TotalMaterials { get; set; }
        public Dictionary<string, int> MaterialsByType { get; set; } = new();
        public long TotalStorageUsed { get; set; }
        public string TotalStorageUsedFormatted { get; set; } = string.Empty;
        public int MaterialsUploadedToday { get; set; }
        public int MaterialsUploadedThisWeek { get; set; }
        public int MaterialsUploadedThisMonth { get; set; }
        public List<TopUploaderDto> TopUploaders { get; set; } = new();
    }

    public class TopUploaderDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int MaterialCount { get; set; }
    }
}
