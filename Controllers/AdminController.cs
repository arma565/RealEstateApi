using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using RealEstate.Authentication;
using RealEstate.Helper;
using RealEstate.Models.Users;
using RealEstate.Services.Images;
using RealEstate.Services.Users.AdminRepository;
using System.Security;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

#pragma warning disable CA1515
#pragma warning disable CA3003
namespace RealEstate.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class AdminController(
    AdminRepositoryService adminService
    ) : ControllerBase
{
    private readonly AdminRepositoryService _adminService = adminService;

    #region AdminServices
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
    #endregion
}


