using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Entities.Users;
using RealEstate.Services.Users;
using RealEstate.Services.Validations;
using System.Security;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;


namespace RealEstate.Controllers.Users;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class AdminController(
    AdminService adminService,
    UserService userService,
    ILogger<AdminController> logger
    ) : ControllerBase
{
    private readonly AdminService _adminService = adminService;

    private readonly UserService _userService = userService;

    private readonly ILogger<AdminController> _logger = logger;

    [HttpPost("promote")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> PromoteUser(string userName)
    {
        try
        {
            var user = await _userService.GetByUserNameAsync(userName).ConfigureAwait(false);

            if (user == null)
                return NotFound("User not found!");

            var promoteResult = await _adminService.PromoteAsync(user).ConfigureAwait(false);

            if (!promoteResult.Succeeded)
                return StatusCode(403, "Promote failed!");

            return Ok("User is admin now");
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
    }

    [HttpGet("users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers() => Ok(await _adminService.GetUsersListAsync().ConfigureAwait(false));

    [HttpDelete("delete-users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteUsers()
    {

        try
        {
            await _adminService.DeleteUsersAsync().ConfigureAwait(false);
            return Ok("Users has been deleted");
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
    }

    [HttpDelete("delete/{userName}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteAdmin(string id)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id);

            var adminUser = await _userService.GetAsync(id).ConfigureAwait(false);

            if (adminUser == null)
                return NotFound("Admin user not found!");

            if (!await _adminService.IsAdmin(adminUser).ConfigureAwait(false))
                return Unauthorized("This user is not an admin!");

            var res = await _userService.DeleteAsync(id).ConfigureAwait(false);

            if (!res.Succeeded)
                return BadRequest(res.Errors);

            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
    }
}


