using Academix.WinApp.Models.Student;
using Academix.WinApp.Models.Teacher;
using System;
using System.Linq;

namespace Academix.WinApp.Utils
{
    public static class ExamShuffleHelper
    {
        public static void ShuffleExam(StartExamResponseDto attempt)
    {
        // Seed cố định theo AttemptId → resume không đổi thứ tự
        var rng = new Random(attempt.AttemptId);

        // Trộn câu hỏi
        attempt.Questions = attempt.Questions
            .OrderBy(q => rng.Next())
            .ToList();

        // Trộn đáp án từng câu
        foreach (var q in attempt.Questions)
        {
            q.Options = q.Options
                .OrderBy(o => rng.Next())
                .ToList();
        }

        // Gán lại QuestionOrder cho UI
        for (int i = 0; i < attempt.Questions.Count; i++)
        {
            attempt.Questions[i].QuestionOrder = i + 1;
        }
    }
    }
}
