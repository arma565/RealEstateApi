using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.DTOs.Users;
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

    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers() => Ok(await _adminService.GetUsersListAsync().ConfigureAwait(false));

    [HttpGet("{userName}")]
    public async Task<ActionResult<ApplicationUser>> Get(string userName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userName))
                return Unauthorized("User name can not be empty!");

            var user = await _adminService.GetByUserNameAsync(userName).ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(user);

            return Ok(user);

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

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterAccountDTO userRegisterAccountDTO)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(userRegisterAccountDTO);

            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            var allUsers = await _adminService.GetUsersListAsync().ConfigureAwait(false);

            if (allUsers.Any(u => u.UserName == userRegisterAccountDTO.UserName))
                return Unauthorized("Username is already taken!");

            if (allUsers.Any(u => u.Email == userRegisterAccountDTO.Email))
                return Unauthorized("Email is already taken!");

            var registerResult = await _adminService.RegisterAsync(userRegisterAccountDTO).ConfigureAwait(false);

            if (!registerResult.Succeeded)
                return Unauthorized(registerResult.Errors);
            else
                return Created();
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

    [HttpDelete("delete")]
    [Authorize(Policy = "AuthenticatedUser")]
    [AllowAnonymous]
    public async Task<IActionResult> Delete([FromBody] LoginRequestDTO userLoginRequestDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            ArgumentNullException.ThrowIfNull(userLoginRequestDTO);

            var result = await _userService.LoginAsync(userLoginRequestDTO).ConfigureAwait(false);

            if (!result.Succeeded)
                return Unauthorized("Invalid username or password.");

            var user = await _adminService.GetByUserNameAsync(userLoginRequestDTO.UserName).ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(user);

            var isAdmin = await _adminService.IsAdmin(user).ConfigureAwait(false);

            if (isAdmin)
                return Unauthorized("This user is an admin!");

            await _adminService.DeleteAsync(user.Id).ConfigureAwait(false);

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

    [HttpPost("promote")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> PromoteUser(string userName)
    {
        try
        {
            var user = await _adminService.GetByUserNameAsync(userName).ConfigureAwait(false);

            if (user == null)
                return NotFound("User not found!");

            var promoteResult = await _adminService.PromoteAsync(userName).ConfigureAwait(false);

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

    [HttpDelete("delete/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteAdmin(string id)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id);

            var adminUser = await _adminService.GetAsync(id).ConfigureAwait(false);

            if (adminUser == null)
                return NotFound("Admin user not found!");

            if (!await _adminService.IsAdmin(adminUser).ConfigureAwait(false))
                return Unauthorized("This user is not an admin!");

            await _adminService.DeleteAsync(id).ConfigureAwait(false);

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


