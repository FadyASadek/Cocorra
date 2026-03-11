using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cocorra.BLL.Services.Upload
{
    public interface IUploadVoice
    {
        Task<string> SaveVoice (IFormFile voiceFile); 
    }
}