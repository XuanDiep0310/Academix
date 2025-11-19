namespace Academix.WinApp.Models.Classes.Responses
{
    public class ClassDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassCode { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TeacherCount { get; set; }
        public int StudentCount { get; set; }
    }
}
