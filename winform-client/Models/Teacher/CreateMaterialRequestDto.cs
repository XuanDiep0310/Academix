using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Teacher
{
    public class CreateMaterialRequestDto
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string MaterialType { get; set; } = "";
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
    }
}
