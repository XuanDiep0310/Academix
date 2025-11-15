using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Teacher
{
    public class UploadMaterialRequestDto
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string FilePath { get; set; } = "";
    }
}
