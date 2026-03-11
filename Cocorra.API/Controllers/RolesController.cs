using Cocorra.BLL.Services.RolesService;
using Cocorra.DAL.AppMetaData;
using Cocorra.DAL.DTOS;
using Cocorra.DAL.DTOS.Role;
using Microsoft.AspNetCore.Mvc;

namespace Cocorra.API.Controllers
{
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet(Router.RolesRouting.GetRoles)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _rolesService.GetRolesAsync();
            return Ok(result);
        }

        [HttpGet(Router.RolesRouting.GetRoleById)]
        public async Task<IActionResult> GetRoleById([FromRoute] string id)
        {
            var result = await _rolesService.GetRoleByIdAsync(id);
            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpPost(Router.RolesRouting.Create)]
        public async Task<IActionResult> CreateRole([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Role name is required.");

            var result = await _rolesService.CreateRoleAsync(name);

            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpPut(Router.RolesRouting.Update)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _rolesService.UpdateRoleAsync(model);

            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete(Router.RolesRouting.Delete)]
        public async Task<IActionResult> DeleteRole([FromRoute] string id)
        {
            var result = await _rolesService.DeleteRoleAsync(id);

            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpPost(Router.RolesRouting.ManageUserRoles)]
        public async Task<IActionResult> ManageUserRoles([FromBody] ManageUserRolesDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _rolesService.ManageUserRolesAsync(model);

            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet(Router.RolesRouting.GetUsersInRole)]
        public async Task<IActionResult> GetUsersInRole([FromRoute] string roleName)
        {
            var result = await _rolesService.GetUsersInRoleAsync(roleName);

            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }
    }
}