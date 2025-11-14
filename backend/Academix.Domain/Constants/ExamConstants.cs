namespace Academix.Domain.Constants
{
    public static class ExamStatus
    {
        public const string InProgress = "InProgress";
        public const string Submitted = "Submitted";
        public const string Graded = "Graded";

        public static readonly string[] All = { InProgress, Submitted, Graded };

        public static bool IsValid(string status)
        {
            return All.Contains(status, StringComparer.OrdinalIgnoreCase);
        }
    }
}
