using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using RealEstate.Authentication;
using RealEstate.Helper;
using RealEstate.Models.Users;
using RealEstate.Services.Images;
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

    /// <summary>
    /// Retrieves all registered users.
    /// </summary>
    /// <returns>Returns a list of all users.</returns>
    [HttpGet("user/users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<User>>> GetUsersAsync() => Ok(await _adminService.GetUsersAsync().ConfigureAwait(false));

    /// <summary>
    /// Deletes all users.
    /// </summary>
    /// <returns>Returns 200 OK if successful, or 500 for errors.</returns>
    [HttpDelete("user/delete-all")]
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
    /// Deletes a user by username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>Returns 204 NoContent if successful, 400 BadRequest for invalid input or incorrect password, 404 NotFound if user not found.</returns>
    [HttpDelete("user/delete/{userName}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteUserAsync(string userName)
    {
        try
        {

            var user = await _userService.GetUserByUserNameAsync(userName).ConfigureAwait(false);

            if (user == null)
                return NotFound("User not found.");

            var res = await _userService.DeleteUserAsync(user!).ConfigureAwait(false);

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


