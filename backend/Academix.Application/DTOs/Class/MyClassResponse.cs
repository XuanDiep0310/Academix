using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Class
{
    public class MyClassResponse
    {
        public int ClassId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CourseId { get; set; }
    }
}
