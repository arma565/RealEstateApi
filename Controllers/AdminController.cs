using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Models.Users;
using RealEstate.Services.Users.AdminRepository;
using RealEstate.Services.Users.UserRepository;
using System.Security;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

#pragma warning disable CA1515
#pragma warning disable CA3003
namespace RealEstate.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class AdminController(
    AdminRepositoryService adminService,
    UserRepositoryService userService
    ) : ControllerBase
{
    private readonly AdminRepositoryService _adminService = adminService;
    private readonly UserRepositoryService _userService = userService;

    [HttpPost("/promote")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> PromoteUser(string userName) {

        var user = await _userService.FindUserByUserNameAsync(userName).ConfigureAwait(false);

        if (user == null)
            return NotFound("User not found!");

        var promoteResult = await _adminService.PromoteUser(user).ConfigureAwait(false);

        if (!promoteResult.Succeeded)
            return StatusCode(500);

        return Ok("User is admin now");
    }

    /// <summary>
    /// Retrieves all registered users.
    /// </summary>
    /// <returns>Returns a list of all users.</returns>
    [HttpGet("/users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<ApplicationUser>>> GetUsersAsync() => Ok(await _adminService.GetUsersAsync().ConfigureAwait(false));

    /// <summary>
    /// Deletes all users.
    /// </summary>
    /// <returns>Returns 200 OK if successful, or 500 for errors.</returns>
    [HttpDelete("/delete-users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteUsersAsync()
    {
        try
        {
            await _adminService.DeleteAllUsersAsync().ConfigureAwait(false);
            return Ok("Users has been deleted");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
    }

    /// <summary>
    /// Delete admin by username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>Returns 204 NoContent if successful, 400 BadRequest for invalid input or incorrect password, 404 NotFound if user not found.</returns>
    [HttpDelete("/delete/{userName}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteAdminAsync(string userName)
    {
        try
        {
            var adminUser = await _userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);

            if (adminUser == null)
                return NotFound("Admin user not found!");

            var isAdmin = await _adminService.IsAdmin(adminUser).ConfigureAwait(false);

            if (!isAdmin)
                return Unauthorized("This user is not an admin!");

            var res = await _userService.DeleteUserAsync(adminUser).ConfigureAwait(false);

            if (!res.Succeeded)
                return BadRequest(res.Errors);

            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
    }
}


