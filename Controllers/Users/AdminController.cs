using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Users;
using RealEstate.Services.Repositories.Users.AdminRepositories;
using RealEstate.Services.Repositories.Users.UserRepositories;
using RealEstate.Services.Validations;
using System.Security;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;


namespace RealEstate.Controllers.Users;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class AdminController(
    AdminRepository adminService,
    UserRepository userService,
    ILogger<AdminController> logger
    ) : ControllerBase
{
    private readonly AdminRepository _adminService = adminService;

    private readonly UserRepository _userService = userService;

    private readonly ILogger<AdminController> _logger = logger;

    [HttpPost("promote")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> PromoteUser(string userName)
    {
        try
        {
            var user = await _userService.FindByUserNameAsync(userName).ConfigureAwait(false);

            if (user == null)
                return NotFound("User not found!");

            var promoteResult = await _adminService.PromoteUserAsync(user).ConfigureAwait(false);

            if (!promoteResult.Succeeded)
                return StatusCode(500);

            return Ok("User is admin now");
        }

        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpGet("users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsersAsync() => Ok(await _adminService.GetUsersListAsync().ConfigureAwait(false));

    [HttpDelete("delete-users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteUsersAsync()
    {

        try
        {
            await _adminService.DeleteUsersAsync().ConfigureAwait(false);
            return Ok("Users has been deleted");
        }

        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpDelete("delete/{userName}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteAdminAsync(string userName)
    {

        try
        {
            var adminUser = await _userService.GetByUserNameAsync(userName).ConfigureAwait(false);

            if (adminUser == null)
                return NotFound("Admin user not found!");

            var isAdmin = await _adminService.IsAdmin(adminUser).ConfigureAwait(false);

            if (!isAdmin)
                return Unauthorized("This user is not an admin!");

            var res = await _userService.DeleteAsync(adminUser).ConfigureAwait(false);

            if (!res.Succeeded)
                return BadRequest(res.Errors);

            return NoContent();
        }

        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }
}


