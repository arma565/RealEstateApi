using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Authentication;
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
public sealed class UserController(
    UserService userService,
    AdminService adminService,
    TokenService tokenService,
     ILogger<UserController> logger
    ) : ControllerBase
{
   
    private readonly UserService _userService = userService;

    private readonly AdminService _adminService = adminService;

    private readonly TokenService _tokenService = tokenService;

    private readonly ILogger<UserController> _logger = logger;

    [HttpPost("login")]
    [Authorize(Policy = "AuthenticatedUser")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO userLoginRequestDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            ArgumentNullException.ThrowIfNull(userLoginRequestDTO);

            var user = await _adminService.GetByUserNameAsync(userLoginRequestDTO.UserName).ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(user);

            var result = await _userService.LoginAsync(userLoginRequestDTO).ConfigureAwait(false);

            if (result.Succeeded)
            {
                var token = await _tokenService.CreateAccessTokenAsync(user).ConfigureAwait(false);

                return Ok(token);
            }

            return Unauthorized("Username or password is not correct! please try again");

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
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO userChangePasswordDTO)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(userChangePasswordDTO);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ChangePasswordAsync(userChangePasswordDTO.UserName, userChangePasswordDTO.OldPassword, userChangePasswordDTO.NewPassword).ConfigureAwait(false);

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
    public async Task<ActionResult<string>> ForgotPassword([FromBody] ForgotPasswordDTO userForgotPasswordDTO)
    { 
        try
        {
            ArgumentNullException.ThrowIfNull(userForgotPasswordDTO);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var generatedToken = await _userService.GenerateTokenToRecoverUserAsync(userForgotPasswordDTO.Email).ConfigureAwait(false);

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
    public async Task<IActionResult> EditProfile(string userId,[FromBody] EditProfileDTO editProfileDTO)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(userId);
            ArgumentNullException.ThrowIfNull(editProfileDTO);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.EditUserProfileAsync(userId , editProfileDTO).ConfigureAwait(false);

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
}


