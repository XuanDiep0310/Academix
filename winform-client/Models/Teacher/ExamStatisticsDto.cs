namespace Academix.WinApp.Models.Teacher
{
    public class ExamStatisticsDto
    {
        public double AverageScore { get; set; }
        public double HighestScore { get; set; }
        public double LowestScore { get; set; }
        public int TotalAttempts { get; set; }
        public int CompletedAttempts { get; set; }
        public int InProgressAttempts { get; set; }
        public double PassRate { get; set; }
    }
}

