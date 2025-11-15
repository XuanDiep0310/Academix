using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Teacher
{
    public class MaterialResponseDto
    {
        public int MaterialId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string MaterialType { get; set; } = "";
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public string? FileSizeFormatted { get; set; }
        public int UploadedBy { get; set; }
        public string? UploadedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


}
