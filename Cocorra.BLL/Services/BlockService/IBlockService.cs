using Cocorra.BLL.Base;
using System;
using System.Threading.Tasks;

namespace Cocorra.BLL.Services.BlockService
{
    public interface IBlockService
    {
        Task<Response<string>> BlockUserAsync(Guid currentUserId, Guid targetUserId);
        Task<Response<string>> UnblockUserAsync(Guid currentUserId, Guid targetUserId);
    }
}
