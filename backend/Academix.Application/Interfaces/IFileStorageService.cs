using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<(bool Success, string? FileUrl, string? FileName, long FileSize, string? ErrorMessage)> UploadFileAsync(
            IFormFile file,
            string folderPath);

        Task<bool> DeleteFileAsync(string fileUrl);

        string GetFileExtension(string fileName);

        bool IsValidFileExtension(string extension);

        bool IsValidFileSize(long fileSize);

        string FormatFileSize(long bytes);

        string DetermineMaterialType(string extension);
    }
}
