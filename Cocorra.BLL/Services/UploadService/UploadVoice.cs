using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Cocorra.BLL.Services.Upload
{
    public class UploadVoice : IUploadVoice
    {
        private readonly IWebHostEnvironment _env;

        private readonly string[] _allowedExtensions = { ".mp3", ".wav", ".m4a", ".ogg", ".aac" };

       
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
                return "Error:ServerException";
            }
        }
    }
}