using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cocorra.BLL.DTOS.Auth;
using Core.Base;

namespace Cocorra.BLL.Services.Auth
{
    public interface IAuthServices
    {
        Task<Response<AuthModel>> LoginAsync(LoginDto dto);
        Task<Response<AuthModel>> RegisterAsync(RegisterDto dto);
    }
}