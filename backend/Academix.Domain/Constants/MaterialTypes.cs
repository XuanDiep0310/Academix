using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Constants
{
    public static class MaterialTypes
    {
        public const string PDF = "PDF";
        public const string Video = "Video";
        public const string Image = "Image";
        public const string Link = "Link";
        public const string Other = "Other";

        public static readonly string[] All = { PDF, Video, Image, Link, Other };

        public static bool IsValid(string type)
        {
            return All.Contains(type, StringComparer.OrdinalIgnoreCase);
        }

        public static string[] AllowedFileExtensions = { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".avi", ".mov", ".mp3", ".wav" };

        public static long MaxFileSize = 50 * 1024 * 1024; // 50 MB
    }
}
