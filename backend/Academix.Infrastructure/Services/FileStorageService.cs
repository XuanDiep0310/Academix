using Academix.Application.Interfaces;
using Academix.Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _uploadPath;

        public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _uploadPath = _configuration["FileStorage:UploadPath"] ?? "wwwroot/uploads";

            // Ensure upload directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<(bool Success, string? FileUrl, string? FileName, long FileSize, string? ErrorMessage)> UploadFileAsync(
            IFormFile file,
            string folderPath)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return (false, null, null, 0, "File is empty");
                }

                // Validate file extension
                var extension = GetFileExtension(file.FileName);
                if (!IsValidFileExtension(extension))
                {
                    return (false, null, null, 0, $"File type {extension} is not allowed");
                }

                // Validate file size
                if (!IsValidFileSize(file.Length))
                {
                    return (false, null, null, 0, $"File size exceeds maximum allowed size of {FormatFileSize(MaterialTypes.MaxFileSize)}");
                }

                // Create folder if not exists
                var fullFolderPath = Path.Combine(_uploadPath, folderPath);
                if (!Directory.Exists(fullFolderPath))
                {
                    Directory.CreateDirectory(fullFolderPath);
                }

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(fullFolderPath, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate URL (relative path for API)
                var fileUrl = $"/uploads/{folderPath}/{uniqueFileName}";

                _logger.LogInformation($"File uploaded successfully: {fileUrl}");
                return (true, fileUrl, file.FileName, file.Length, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                return (false, null, null, 0, "Error uploading file");
            }
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return true;
                }

                // Convert URL to physical path
                var relativePath = fileUrl.TrimStart('/');
                var filePath = Path.Combine(_uploadPath.Replace("wwwroot/", ""), relativePath.Replace("uploads/", ""));
                filePath = Path.Combine("wwwroot", filePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"File deleted successfully: {fileUrl}");
                    return true;
                }

                return true; // File doesn't exist, consider it deleted
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting file {fileUrl}: {ex.Message}");
                return false;
            }
        }

        public string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName).ToLower();
        }

        public bool IsValidFileExtension(string extension)
        {
            return MaterialTypes.AllowedFileExtensions.Contains(extension.ToLower());
        }

        public bool IsValidFileSize(long fileSize)
        {
            return fileSize <= MaterialTypes.MaxFileSize;
        }

        public string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        public string DetermineMaterialType(string extension)
        {
            return extension.ToLower() switch
            {
                ".pdf" => MaterialTypes.PDF,
                ".jpg" or ".jpeg" or ".png" or ".gif" => MaterialTypes.Image,
                ".mp4" or ".avi" or ".mov" or ".wmv" or ".flv" => MaterialTypes.Video,
                ".doc" or ".docx" or ".ppt" or ".pptx" or ".xls" or ".xlsx" => MaterialTypes.Other,
                _ => MaterialTypes.Other
            };
        }
    }
}
