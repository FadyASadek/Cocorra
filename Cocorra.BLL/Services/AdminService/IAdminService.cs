using Cocorra.DAL.DTOS.AdminDto;
using Cocorra.DAL.Enums;
using Core.Base;


namespace Cocorra.BLL.Services.AdminService
{
    public interface IAdminService
    {
        Task<Response<IEnumerable<UserDto>>> GetAllUsersAsync(string? search, int page = 1, int pageSize = 10);
        Task<Response<UserDto>> GetUserByIdAsync(Guid userId);
        Task<Response<DashboardStatsDto>> GetDashboardStatsAsync();

       
        Task<Response<string>> ChangeUserStatusAsync(Guid userId, UserStatus newStatus);

        Task<Response<UserDto>> UpdateUserAsync(Guid userId, UpdateUserDto model);

        Task<Response<string>> DeleteUserAsync(Guid userId);

        Task<Response<string>> ResetUserPasswordAsync(Guid userId, string newPassword);
    }
}
