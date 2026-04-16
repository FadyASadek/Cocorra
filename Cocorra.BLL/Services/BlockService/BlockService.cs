using Cocorra.BLL.Base;
using Cocorra.DAL.Repository.UserBlockRepository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Cocorra.DAL.Models;

namespace Cocorra.BLL.Services.BlockService
{
    public class BlockService : ResponseHandler, IBlockService
    {
        private readonly IUserBlockRepository _blockRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlockService(IUserBlockRepository blockRepo, UserManager<ApplicationUser> userManager)
        {
            _blockRepo = blockRepo;
            _userManager = userManager;
        }

        public async Task<Response<string>> BlockUserAsync(Guid currentUserId, Guid targetUserId)
        {
            if (currentUserId == targetUserId)
                return BadRequest<string>("You cannot block yourself.");

            var targetUser = await _userManager.FindByIdAsync(targetUserId.ToString());
            if (targetUser == null) return NotFound<string>("Target user not found.");

            await _blockRepo.BlockUserAsync(currentUserId, targetUserId);

            return Success("User blocked successfully.");
        }

        public async Task<Response<string>> UnblockUserAsync(Guid currentUserId, Guid targetUserId)
        {
            var targetUser = await _userManager.FindByIdAsync(targetUserId.ToString());
            if (targetUser == null) return NotFound<string>("Target user not found.");

            await _blockRepo.UnblockUserAsync(currentUserId, targetUserId);

            return Success("User unblocked successfully.");
        }
    }
}
