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

    [HttpGet("{userName}")]
    public async Task<ActionResult<ApplicationUser>> Get(string userName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userName))
                return Unauthorized("User name can not be empty!");

            var user = await _userService.GetByUserNameAsync(userName).ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(user);

            return Ok(user);

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

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegisterAccountDTO userRegisterAccountDTO)
    {
        try
        {
            bool isFirstUser = false;

            ArgumentNullException.ThrowIfNull(userRegisterAccountDTO);

            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            var allUsers = await _adminService.GetUsersListAsync().ConfigureAwait(false);

            if (!allUsers.Any()) 
                isFirstUser = true;
            
            if (allUsers.Any(u => u.UserName == userRegisterAccountDTO.UserName))
                return Unauthorized("Username is already taken!");

            if (allUsers.Any(u => u.Email == userRegisterAccountDTO.Email))
                return Unauthorized("Email is already taken!");

            var registerResult = await _userService.RegisterAsync(userRegisterAccountDTO).ConfigureAwait(false);

            if (!registerResult.Succeeded)
                return Unauthorized(registerResult.Errors);
            else
            {
                if (isFirstUser) {
                    var registeredUser = await _userService.GetByUserNameAsync(userRegisterAccountDTO.UserName).ConfigureAwait(false);
                    ArgumentNullException.ThrowIfNull(registeredUser);
                    await _adminService.PromoteAsync(registeredUser).ConfigureAwait(false);
                }
                return Created();
            }
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

    [HttpPost("login")]
    [Authorize(Policy = "AuthenticatedUser")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO userLoginRequestDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            ArgumentNullException.ThrowIfNull(userLoginRequestDTO);

            var user = await _userService.GetByUserNameAsync(userLoginRequestDTO.UserName).ConfigureAwait(false);

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

    [HttpDelete("delete")]
    [Authorize(Policy = "AuthenticatedUser")]
    [AllowAnonymous]
    public async Task<IActionResult> Delete([FromBody] UserLoginRequestDTO userLoginRequestDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            ArgumentNullException.ThrowIfNull(userLoginRequestDTO);

            var result = await _userService.LoginAsync(userLoginRequestDTO).ConfigureAwait(false);

            if (!result.Succeeded)
                return Unauthorized("Invalid username or password.");

            var user = await _userService.FindByUserNameAsync(userLoginRequestDTO.UserName).ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(user);

            var isAdmin = await _adminService.IsAdmin(user).ConfigureAwait(false);

            if (isAdmin)
                return Unauthorized("This user is an admin!");

            var res = await _userService.DeleteAsync(user.Id).ConfigureAwait(false);

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

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordDTO userResetPasswordDTO)
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

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDTO userChangePasswordDTO)
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

    [HttpPost("forgot-password")]
    public async Task<ActionResult<string>> ForgotPassword([FromBody] UserForgotPasswordDTO userForgotPasswordDTO)
    { 
        try
        {
            ArgumentNullException.ThrowIfNull(userForgotPasswordDTO);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //if (string.IsNullOrWhiteSpace(userForgotPasswordDTO.Email))
            //    return BadRequest("Email is required!");

            //if (!new EmailHelper().IsValidEmail(userForgotPasswordDTO.Email))
            //    return BadRequest("Invalid email format.");

            var generatedToken = await _userService.GenerateTokenToRecoverUserAsync(userForgotPasswordDTO.Email).ConfigureAwait(false);

            return Ok(generatedToken);
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

    [HttpPut("edit-profile/{userId}")]
    public async Task<IActionResult> EditProfile(string userId,[FromBody] ApplicationUser applicationUser)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(userId);
            ArgumentNullException.ThrowIfNull(applicationUser);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.EditUserProfileAsync(userId , applicationUser).ConfigureAwait(false);

            if (result.Succeeded)
                return Ok("User profile has been updated");
            else
                return BadRequest(result.Errors);
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


