using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Materials
{
    public class MaterialResponseDto
    {
        public int MaterialId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public string? FileSizeFormatted { get; set; }
        public int UploadedBy { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
