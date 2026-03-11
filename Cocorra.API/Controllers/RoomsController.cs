using Cocorra.BLL.Services.RoomService;
using Cocorra.DAL.AppMetaData;
using Cocorra.DAL.DTOS.RoomDto;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cocorra.API.Controllers;

public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost(Router.RoomRouting.Create)] // أو خليها "api/rooms" لو مفيش راوتر
    public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
    {
        // 1. هنجيب الـ ID بتاع اليوزر اللي باعت الطلب من التوكن
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid hostId))
        {
            return Unauthorized("User ID is invalid or missing.");
        }

        // 2. ننادي السيرفيس
        var result = await _roomService.CreateRoomAsync(dto, hostId);

        // 3. نرجع النتيجة
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}
