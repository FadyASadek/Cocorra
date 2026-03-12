using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cocorra.BLL.Services.Upload
{
    public class UploadImage : IUploadImage
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024; 

        public UploadImage(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0) return "Error:NoFile";
                if (imageFile.Length > _maxFileSize) return "Error:FileTooLarge";

                string extension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension)) return "Error:InvalidExtension";

                if (!imageFile.ContentType.StartsWith("image/")) return "Error:InvalidFileType";

                string contentPath = string.IsNullOrWhiteSpace(_env.WebRootPath)
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : _env.WebRootPath;

                string path = Path.Combine(contentPath, "Uploads", "Images", "Profiles");

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                string fileName = Guid.NewGuid().ToString() + extension;
                string fullPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return Path.Combine("Uploads", "Images", "Profiles", fileName).Replace("\\", "/");
            }
            catch (Exception)
            {
                return "Error:ServerException";
            }
        }

        public void DeleteImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            try
            {
                string contentPath = string.IsNullOrWhiteSpace(_env.WebRootPath)
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : _env.WebRootPath;

                var fullPath = Path.Combine(contentPath, imagePath.Replace("/", "\\"));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
            }
        }
    }
}