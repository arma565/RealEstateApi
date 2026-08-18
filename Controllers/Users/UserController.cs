using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.DTOs.Users;
using RealEstate.Entities.Users;
using RealEstate.Services.Users;
using RealEstate.Services.Users.Authentications;
using RealEstate.Services.Validations;
using System.Security;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace RealEstate.Controllers.Users;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class UserController(
    UserService userService,
    TokenService tokenService,
     ILogger<UserController> logger
    ) : ControllerBase
{
    private readonly UserService _userService = userService;
    private readonly TokenService _tokenService = tokenService;
    private readonly ILogger<UserController> _logger = logger;

    [HttpGet("get-list")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAll() => Ok(await _userService.GetUsersListAsync().ConfigureAwait(false));

    [HttpGet("get/{userName}")]
    public async Task<ActionResult<ApplicationUser>> Get(string userName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest("Username can not be empty!");

            var user = await _userService.GetByUserNameAsync(userName).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(user);

            return user is not null ? Ok(user) : NotFound("No such user found!");
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
    public async Task<IActionResult> Register([FromBody] RegisterAccountDTO userRegisterAccountDTO)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(userRegisterAccountDTO);

            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            var registerResult = await _userService.RegisterAsync(userRegisterAccountDTO).ConfigureAwait(false);

            if (!registerResult.Succeeded)
                return StatusCode(400,registerResult.Errors);
            else 
                return Created();
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, ex.Message);
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

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO userLoginRequestDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            ArgumentNullException.ThrowIfNull(userLoginRequestDTO);

            return Ok(await _userService.LoginAsync(userLoginRequestDTO).ConfigureAwait(false));
        }
        catch (InvalidOperationException ex) {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, ex.Message);
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

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            return Ok(await _tokenService.RefreshToken(request).ConfigureAwait(false));
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

    [HttpPost("reset-password")]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO userResetPasswordDTO)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(userResetPasswordDTO);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ResetPasswordAsync(userResetPasswordDTO.Email, userResetPasswordDTO.Token, userResetPasswordDTO.NewPassword).ConfigureAwait(false);

            if (result.Succeeded)
                return Ok("Reset password  was successful");

            return BadRequest(result.Errors);
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

    [HttpPost("change-password")]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO userChangePasswordDTO)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(userChangePasswordDTO);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService
                .ChangePasswordAsync(
                userChangePasswordDTO.UserName,
                userChangePasswordDTO.OldPassword,
                userChangePasswordDTO.NewPassword).ConfigureAwait(false);

            if (result.Succeeded)
                return Ok("Password has been changed");

            return BadRequest(result.Errors);

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

    [HttpPost("forgot-password")]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<ActionResult<string>> ForgotPassword([FromBody] ForgotPasswordDTO userForgotPasswordDTO)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(userForgotPasswordDTO);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var generatedToken = await _tokenService.GeneratePasswordResetTokenAsync(userForgotPasswordDTO.Email).ConfigureAwait(false);

            return Ok(generatedToken);
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

    [HttpPut("edit-profile/{userId}")]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<IActionResult> EditProfile(string userId, [FromBody] EditProfileDTO editProfileDTO)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(userId);
            ArgumentNullException.ThrowIfNull(editProfileDTO);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.EditUserProfileAsync(userId, editProfileDTO).ConfigureAwait(false);

            if (result.Succeeded)
                return Ok("User profile has been updated");
            else
                return BadRequest(result.Errors);
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

    [HttpPost("promote/{userName}")]
    [Authorize(Policy = "ManagerOnly")]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<IActionResult> PromoteUser(string userName)
    {
        try
        {
            var promoteResult = await _userService.PromoteAsync(userName).ConfigureAwait(false);

            if (!promoteResult.Succeeded)
                return StatusCode(400, "Promote failed!");

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

    [HttpPost("demote/{userName}")]
    [Authorize(Policy = "ManagerOnly")]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<IActionResult> DemoteUser(string userName)
    {
        try
        {
            var demoteResult = await _userService.DemoteAsync(userName).ConfigureAwait(false);

            if (!demoteResult.Succeeded)
                return StatusCode(400, "Demote failed!");

            return Ok("User is agent now");
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
    [Authorize(Policy = "AuthenticatedUser")]
    [AllowAnonymous]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _userService.DeleteAsync(id).ConfigureAwait(false);

            return NoContent();

        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return BadRequest(ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return BadRequest("Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return Forbid("Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
    }

    [HttpDelete("delete-admin/{id}")]
    [Authorize(Policy = "ManagerOnly")]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<IActionResult> DeleteAdmin(string id)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id);

            var adminUser = await _userService.GetAsync(id).ConfigureAwait(false);

            if (adminUser == null)
                return NotFound("Admin user not found!");

            if (!await _userService.IsAdmin(adminUser).ConfigureAwait(false))
                return Unauthorized("This user is not an admin!");

            if (adminUser.UserName == "Admin")
                return Unauthorized("Default admin user can not be deleted!");

            await _userService.DeleteAsync(id).ConfigureAwait(false);

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

    [HttpDelete("delete-users")]
    [Authorize(Policy = "AdminOrManager")]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<IActionResult> DeleteUsers()
    {

        try
        {
            await _userService.DeleteUsersAsync().ConfigureAwait(false);
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
}


