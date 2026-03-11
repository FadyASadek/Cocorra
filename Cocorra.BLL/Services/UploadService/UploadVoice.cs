using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq; // محتاجين دي عشان الـ Contains
using System.Threading.Tasks;

namespace Cocorra.BLL.Services.Upload
{
    public class UploadVoice : IUploadVoice
    {
        private readonly IWebHostEnvironment _env;

        // 1. تحديد قائمة الامتدادات المسموحة فقط (Audio Only)
        private readonly string[] _allowedExtensions = { ".mp3", ".wav", ".m4a", ".ogg", ".aac" };

        // 2. تحديد الحجم الأقصى (3 ميجا بايت)
        // 3 * 1024 * 1024 bytes
        private const long _maxFileSize = 3 * 1024 * 1024;

        public UploadVoice(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveVoice(IFormFile voiceFile)
        {
            try
            {
                if (voiceFile == null || voiceFile.Length == 0)
                {
                    return "Error:NoFile";
                }

                if (voiceFile.Length > _maxFileSize)
                {
                    return "Error:FileTooLarge"; 
                }

                string extension = Path.GetExtension(voiceFile.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    return "Error:InvalidExtension"; 
                }

                
                if (!voiceFile.ContentType.StartsWith("audio/"))
                {
                    return "Error:InvalidFileType";
                }

                string contentPath = _env.WebRootPath;
                string path = Path.Combine(contentPath, "Uploads", "Voices");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Guid.NewGuid().ToString() + extension;
                string fullPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await voiceFile.CopyToAsync(stream);
                }

                return Path.Combine("Uploads", "Voices", fileName).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                // سجل الخطأ هنا لو عندك Logger
                return "Error:ServerException";
            }
        }
    }
}