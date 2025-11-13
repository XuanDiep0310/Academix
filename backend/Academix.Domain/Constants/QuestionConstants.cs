using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Constants
{
    public static class QuestionTypes
    {
        public const string MultipleChoice = "MultipleChoice";
        public const string TrueFalse = "TrueFalse";
        public const string SingleChoice = "SingleChoice";

        public static readonly string[] All = { MultipleChoice, TrueFalse, SingleChoice };

        public static bool IsValid(string type)
        {
            return All.Contains(type, StringComparer.OrdinalIgnoreCase);
        }
    }

    public static class DifficultyLevels
    {
        public const string Easy = "Easy";
        public const string Medium = "Medium";
        public const string Hard = "Hard";

        public static readonly string[] All = { Easy, Medium, Hard };

        public static bool IsValid(string level)
        {
            return All.Contains(level, StringComparer.OrdinalIgnoreCase);
        }
    }
}
