using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Cocorra.BLL.Services.Upload
{
    public interface IUploadImage
    {
        Task<string> SaveImageAsync(IFormFile imageFile);
        void DeleteImage(string? imagePath); 
    }
}