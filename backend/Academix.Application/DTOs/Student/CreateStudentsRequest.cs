using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Student
{
    public class CreateStudentsRequest
    {
        public int ClassId { get; set; }

        // Danh sách học sinh cần thêm
        public List<CreateStudentRequest> Students { get; set; } = new();
    }
}
