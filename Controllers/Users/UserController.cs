using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Authentication;
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
public sealed class UserController(
    UserRepository userService,
    AdminRepository adminService,
    TokenService tokenService,
     ILogger<UserController> logger
    ) : ControllerBase
{
   
    private readonly UserRepository _userService = userService;

    private readonly AdminRepository _adminService = adminService;

    private readonly TokenService _tokenService = tokenService;

    private readonly ILogger<UserController> _logger = logger;

    [HttpGet("{userName}")]
    public async Task<ActionResult<ApplicationUser>> GetAsync(string userName)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest("User name can not be empty!");

            var user = await _userService.GetByUserNameAsync(userName).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

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
    public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterAccountDTO userRegisterAccountDTO)
    {
        try
        {
            if (userRegisterAccountDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            var allUsers = await _adminService.GetUsersListAsync().ConfigureAwait(false);

            if (allUsers.Any(u => u.UserName == userRegisterAccountDTO.UserName))
                return BadRequest("Username is already taken!");

            if (allUsers.Any(u => u.Email == userRegisterAccountDTO.Email))
                return BadRequest("Email is already taken!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var registerResult = await _userService.RegisterAsync(userRegisterAccountDTO).ConfigureAwait(false);

            if (!registerResult.Succeeded)
                return BadRequest(registerResult.Errors);

            var registeredUser = await _userService.FindByUserNameAsync(userRegisterAccountDTO.UserName).ConfigureAwait(false);
            await _adminService.AssignRoleAsync(registeredUser!).ConfigureAwait(false);

            return Created();

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
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequestDTO userLoginRequestDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            if (userLoginRequestDTO == null || userLoginRequestDTO.UserName == null || userLoginRequestDTO.Password == null)
                return Unauthorized("Username and password are required.");

            var user = await _userService.GetByUserNameAsync(userLoginRequestDTO.UserName).ConfigureAwait(false);

            if (user == null)
                return NotFound("User not found.");

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
    public async Task<IActionResult> DeleteAsync([FromBody] UserLoginRequestDTO userLoginRequestDTO)
    {
        try
        {
            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            if (userLoginRequestDTO == null || userLoginRequestDTO.UserName == null || userLoginRequestDTO.Password == null)
                return Unauthorized("Username and password are required.");

            var result = await _userService.LoginAsync(userLoginRequestDTO).ConfigureAwait(false);

            if (!result.Succeeded)
                return Unauthorized("Invalid username or password.");

            var user = await _userService.FindByUserNameAsync(userLoginRequestDTO.UserName).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

            var isAdmin = await _adminService.IsAdmin(user).ConfigureAwait(false);

            if (isAdmin)
                return Unauthorized("This user is an admin!");

            var res = await _userService.DeleteAsync(user).ConfigureAwait(false);

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
    public async Task<IActionResult> ResetPasswordAsync([FromBody] UserResetPasswordDTO userResetPasswordDTO)
    {
        try
        {
            if (userResetPasswordDTO == null)
                return BadRequest("Failed to retreive parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.FindByEmailAsync(userResetPasswordDTO.Email).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

            var result = await _userService.ResetPasswordAsync(user, userResetPasswordDTO.Token, userResetPasswordDTO.NewPassword).ConfigureAwait(false);

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
    public async Task<IActionResult> ChangePasswordAsync([FromBody] UserChangePasswordDTO userChangePasswordDTO)
    {
        try
        {
            if (userChangePasswordDTO == null)
                return BadRequest("Failed to retreive parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.FindByUserNameAsync(userChangePasswordDTO.UserName).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

            var result = await _userService.ChangePasswordAsync(user, userChangePasswordDTO.OldPassword, userChangePasswordDTO.NewPassword).ConfigureAwait(false);

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
    public async Task<ActionResult<string>> ForgotPasswordAsync([FromBody] UserForgotPasswordDTO userForgotPasswordDTO)
    { 
        try
        {
            if (userForgotPasswordDTO == null)
                return BadRequest("Failed to retreive parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //if (string.IsNullOrWhiteSpace(userForgotPasswordDTO.Email))
            //    return BadRequest("Email is required!");

            //if (!new EmailHelper().IsValidEmail(userForgotPasswordDTO.Email))
            //    return BadRequest("Invalid email format.");

            var user = await _userService.FindByEmailAsync(userForgotPasswordDTO.Email).ConfigureAwait(false);

            if (user == null)
                return BadRequest("No such user found!");

            var generatedToken = await _userService.GenerateTokenToRecoverUserAsync(user).ConfigureAwait(false);

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

    [HttpPut("edit-profile")]
    public async Task<IActionResult> EditUserProfile([FromBody] ApplicationUser applicationUser)
    {
        try
        {
            if (applicationUser == null)
                return BadRequest("User is null!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.FindByIDAsync(applicationUser.Id).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

            if (string.IsNullOrEmpty(applicationUser.UserName) || string.IsNullOrEmpty(applicationUser.Email))
            {
                user.UserName = user.UserName;
                user.Email = user.Email;
            }
            else
            {
                user.UserName = applicationUser.UserName;
                user.Email = applicationUser.Email;
            }

            user.Id = applicationUser.Id;
            user.FirstName = applicationUser.FirstName;
            user.LastName = applicationUser.LastName;
            user.PhoneNumber = applicationUser.PhoneNumber;
            user.AcceptTerms = user.AcceptTerms;
            user.ImageId = applicationUser.ImageId;

            var result = await _userService.EditUserProfileAsync(user).ConfigureAwait(false);

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


