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
                
                if (!IsValidVoiceSignature(voiceFile)) return "Error:FakeVoice";

                string contentPath = string.IsNullOrWhiteSpace(_env.WebRootPath)
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : _env.WebRootPath;
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
            catch (Exception)
            {
                return "Error:ServerException";
            }
        }

        private bool IsValidVoiceSignature(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    byte[] headerBytes = new byte[12];
                    int bytesRead = stream.Read(headerBytes, 0, headerBytes.Length);
                    stream.Position = 0;
                    
                    if (bytesRead < 2) return false;

                    // MP3, WAV, OGG, raw AAC — fixed-length prefix signatures
                    var signatures = new List<byte[]>
                    {
                        new byte[] { 0x49, 0x44, 0x33 }, // MP3 (ID3)
                        new byte[] { 0xFF, 0xFB }, // MP3 (MPEG audio frame)
                        new byte[] { 0xFF, 0xF3 }, // MP3 (MPEG audio frame)
                        new byte[] { 0xFF, 0xF2 }, // MP3 (MPEG audio frame)
                        new byte[] { 0x52, 0x49, 0x46, 0x46 }, // WAV (RIFF)
                        new byte[] { 0x4F, 0x67, 0x67, 0x53 }, // OGG
                        new byte[] { 0xFF, 0xF1 }, // AAC (ADTS)
                        new byte[] { 0xFF, 0xF9 }  // AAC (ADTS alt)
                    };

                    if (signatures.Any(sig => headerBytes.Take(sig.Length).SequenceEqual(sig)))
                        return true;

                    // M4A / AAC-in-MP4 container: the "ftyp" marker sits at offset 4
                    // regardless of the box size byte at offset 0-3 (varies by encoder).
                    if (bytesRead >= 8)
                    {
                        byte[] ftypMarker = { 0x66, 0x74, 0x79, 0x70 }; // "ftyp"
                        if (headerBytes.Skip(4).Take(4).SequenceEqual(ftypMarker))
                            return true;
                    }

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void DeleteVoice(string? voicePath)
        {
            if (string.IsNullOrEmpty(voicePath)) return;

            try
            {
                string contentPath = string.IsNullOrWhiteSpace(_env.WebRootPath)
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : _env.WebRootPath;

                var fullPath = Path.Combine(contentPath, voicePath.Replace("/", "\\"));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch { }
        }
    }
}