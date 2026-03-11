using Cocorra.DAL.DTOS.RoomDto;
using Core.Base;

namespace Cocorra.BLL.Services.RoomService;

public interface IRoomService
{
    Task<Response<Guid>> CreateRoomAsync(CreateRoomDto dto, Guid hostId);
}
