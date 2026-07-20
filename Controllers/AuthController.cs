using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using RealEstate.Authentication;
using RealEstate.Helper;
using RealEstate.Models.Authentication;
using RealEstate.Models.Authentication.Users;
using RealEstate.Services;
using System.Security;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

#pragma warning disable CA1515
#pragma warning disable CA3003
namespace RealEstate.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class AuthController(
    RepositoryService service,
    TokenService tokenService,
    ImageService imageService
    ) : ControllerBase
{
    private readonly RepositoryService _service = service;

    private readonly TokenService _tokenService = tokenService;

    private readonly ImageService _imageService = imageService;

    #region Auth

    #region ProfileImage
    /// <summary>
    /// Uploads a profile image for the specified user.
    /// </summary>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="image">The image file to upload.</param>
    /// <returns>Returns 200 OK if successful, 400 BadRequest for invalid input, 404 NotFound if user not found, or 500/403 for errors.</returns>
    [HttpPost("user/upload/{userID}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> UploadProfileImage(string userID, IFormFile image)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest("UserID can not be empty!");

            var user = await _service.GetUserByIDAsync(userID).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

            if (image == null)
                return BadRequest("Image can not be empty!");

            var imageFileName = await _imageService.UploadProfileImage(image).ConfigureAwait(false);

            var profileImage = new ProfileImage();

            if (user.ProfileImage != null)
            {
                profileImage.Id = user.ProfileImage!.Id;
                profileImage.ProfileImageName = imageFileName;
                profileImage.UserID = userID;
                await _service.UpdateProfileImageAsync(profileImage).ConfigureAwait(false);
            }
            else
            {
                profileImage.ProfileImageName = imageFileName;
                profileImage.UserID = userID;
                await _service.AddProfileImageAsync(profileImage).ConfigureAwait(false);
            }

            return Ok("ProfileImage successfully uploaded");
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
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
    /// Downloads the profile image for the specified user.
    /// </summary>
    /// <param name="imageFileName">The file name of the image to download.</param>
    /// <returns>Returns the image file if found, 404 NotFound if not found, 400 BadRequest for invalid input, or 500/403 for errors.</returns>
    [HttpGet("user/download/{imageFileName}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> DownloadProfileImage(string imageFileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageFileName))
                return BadRequest("Image file name is empty!");

            List<ProfileImage> profileImagesList = await _service.GetUserProfileImageListAsync().ConfigureAwait(false);

            var profileImage = profileImagesList.FirstOrDefault(profileImg => profileImg.ProfileImageName == imageFileName);

            if (profileImage == null)
                return NotFound("No such image found!");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(profileImage.ProfileImageName);
            if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                return BadRequest("Unsupported image file type.");

            var environmentPath = _imageService.GetLocalImagesFullPath("auth");

            var fullPath = Path.Combine(environmentPath, profileImage.ProfileImageName);

            var normalizedPath = Path.GetFullPath(fullPath);
            var basePath = Path.GetFullPath(environmentPath);
            if (!normalizedPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid file path access.");

            if (!System.IO.File.Exists(fullPath))
                return NotFound("Image file not found!");

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fullPath, out var contentType))
                contentType = "application/octet-stream";

            return PhysicalFile(fullPath, contentType, Path.GetFileName(fullPath));
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
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
    /// Deletes a profile image by its ID.
    /// </summary>
    /// <param name="profileImageID">The GUID of the profile image to delete.</param>
    /// <returns>
    /// Returns 200 OK if the image was successfully deleted,
    /// 400 BadRequest if the ID is invalid,
    /// 404 NotFound if the image does not exist,
    /// or 500/403 for errors.
    /// </returns>
    [HttpDelete("profile/delete/{profileImageID}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> DeleteProfileImage(string profileImageID)
    {
        try
        {
            if (!Guid.TryParse(profileImageID, out Guid realEstateProfileImageID))
                return BadRequest("Id must be a valid GUID!");

            ProfileImage? profileImage = await _service.GetProfileImageAsync(realEstateProfileImageID).ConfigureAwait(false);

            if (profileImage is null)
                return NotFound("Image not found!");

            await _service.DeleteProfileImageAsync(profileImage).ConfigureAwait(false);

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
    #endregion

    /// <summary>
    /// Retrieves all registered users.
    /// </summary>
    /// <returns>Returns a list of all users.</returns>
    [HttpGet("user/users")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<User>>> GetUsers() => Ok(await _service.GetUsersAsync().ConfigureAwait(false));

    /// <summary>
    /// Retrieves a user by their userName.
    /// </summary>
    /// <param name="userName">The userName of the user.</param>
    /// <returns>Returns the user if found, 404 NotFound if not found, or 400 BadRequest for invalid input.</returns>
    [HttpGet("user/{userName}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<ActionResult<User>> GetUserByUserName(string userName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest("User name can not be empty!");

            var user = await _service.GetUserByUserNameAsync(userName).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

            return Ok(user);

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
    /// Registers a new user.
    /// </summary>
    /// <param name="registerUserModel">The registration model containing user details.</param>
    /// <returns>Returns 200 OK if successful, 400 BadRequest for validation errors or if username/email is taken.</returns>
    [HttpPost("user/register")]
    [Authorize(Policy = "AdminOrUser")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUser registerUserModel)
    {
        try
        {
            if (registerUserModel == null)
                return BadRequest("Failed to retreive parameter!");

            var allUsers = await _service.GetUsersAsync().ConfigureAwait(false);

            if (allUsers.Any(u => u.UserName == registerUserModel.UserName))
                return BadRequest("Username is already taken!");

            if (allUsers.Any(u => u.Email == registerUserModel.Email))
                return BadRequest("Email is already taken!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var res = await _service.RegisterUserAsync(registerUserModel).ConfigureAwait(false);

            if (res.Succeeded)
                return CreatedAtAction(nameof(GetUserByUserName), new { userName = registerUserModel.UserName }, registerUserModel);

            return BadRequest(res.Errors);
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
    /// Login a user.
    /// </summary>
    /// <param name="loginUserModel">The userLoginInfo containing username and password.</param>
    /// <returns>Returns 200 OK if successful, 401 Unauthorized if credentials are incorrect, or 400 BadRequest for validation errors.</returns>
    [HttpPost("user/login")]
    [Authorize(Policy = "AuthenticatedUser")]
    [Authorize(Policy = "AdminOrUser")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginUser([FromBody] LoginUser loginUserModel)
    {
        try
        {
            if (!ModelState.IsValid)
                return Unauthorized(ModelState);

            if (loginUserModel == null || loginUserModel.UserName == null || loginUserModel.Password == null)
                return Unauthorized("Username and password are required.");

            var result = await _service.LoginUserAsync(loginUserModel).ConfigureAwait(false);

            if (result.Succeeded) {

                var user = await _service.GetUserByUserNameAsync(loginUserModel.UserName).ConfigureAwait(false);
                var token = await _tokenService.CreateAccessTokenAsync(user!).ConfigureAwait(false);

                return Ok(token);
            }
               

            return Unauthorized("Username or password is not correct! please try again");
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
    /// Deletes all users.
    /// </summary>
    /// <returns>Returns 200 OK if successful, or 500 for errors.</returns>
    [HttpDelete("user/delete-all")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteUsers()
    {
        try
        {
            await _service.DeleteAllUsersAsync().ConfigureAwait(false);
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
    /// <param name="userName">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>Returns 204 NoContent if successful, 400 BadRequest for invalid input or incorrect password, 404 NotFound if user not found.</returns>
    [HttpDelete("user/delete/{userName}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> DeleteUser(string userName)
    {
        try
        {
            var user = await _service.GetUserByUserNameAsync(userName).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

            var res = await _service.DeleteUserAsync(user).ConfigureAwait(false);

            if (res.Succeeded)
                return NoContent();

            return BadRequest(res.Errors);
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
    /// Generates a recovery token for a user to reset their account.
    /// </summary>
    /// <param name="recoverAccountModel">The recovery model containing the user's email.</param>
    /// <returns>Returns the generated token if successful, 400 BadRequest or 404 NotFound for errors.</returns>
    [HttpPost("user/recover/account")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<ActionResult<string>> RecoverAccount([FromBody] RecoverAccount recoverAccountModel)
    {
        try
        {
            if (recoverAccountModel == null)
                return BadRequest("Failed to retreive parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(recoverAccountModel.Email))
                return BadRequest("Email is required!");

            if (!new EmailHelper().IsValidEmail(recoverAccountModel.Email))
                return BadRequest("Invalid email format.");

            var user = await _service.FindUserByEmailAsync(recoverAccountModel.Email).ConfigureAwait(false);

            if (user == null)
                return BadRequest("No such user found!");

            var generatedToken = await _service.GenerateTokenToRecoverUserAsync(user).ConfigureAwait(false);

            return Ok(generatedToken);
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
    /// Resets the password for a user using a token.
    /// </summary>
    /// <param name="resetPasswordModel">The reset model containing email, token, and new password.</param>
    /// <returns>Returns 200 OK if successful, 400 BadRequest or 404 NotFound for errors.</returns>
    [HttpPost("user/reset/password")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPassword resetPasswordModel)
    {
        try
        {
            if (resetPasswordModel == null)
                return BadRequest("Failed to retreive parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _service.FindUserByEmailAsync(resetPasswordModel.Email).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

            var result = await _service.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.NewPassword).ConfigureAwait(false);

            if (result.Succeeded)
                return Ok("Reset password  was successful");

            return BadRequest(result.Errors);
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
    /// Changes the password for a user.
    /// </summary>
    /// <param name="changePasswordModel">The change model containing username, current password, and new password.</param>
    /// <returns>Returns 200 OK if successful, 400 BadRequest or 404 NotFound for errors.</returns>
    [HttpPost("user/change/password")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePassword changePasswordModel)
    {
        try
        {
            if (changePasswordModel == null)
                return BadRequest("Failed to retreive parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _service.FindUserByUserNameAsync(changePasswordModel.UserName).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

            var result = await _service.ChangePasswordAsync(user, changePasswordModel.OldPassword, changePasswordModel.NewPassword).ConfigureAwait(false);

            if (result.Succeeded)
                return Ok("Password has been changed");

            return BadRequest(result.Errors);
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
    /// Edits the profile of a user.
    /// </summary>
    /// <param name="updateUserProfileModel">The user model with updated information.</param>
    /// <returns>Returns 200 OK if successful, 400 BadRequest or 404 NotFound for errors.</returns>
    [HttpPut("user/edit/profile")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> EditUserProfile([FromBody] User editUserProfileModel)
    {
        try
        {
            if (editUserProfileModel == null)
                return BadRequest("User is null!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _service.FindUserByIDAsync(editUserProfileModel.Id).ConfigureAwait(false);

            if (user == null)
                return NotFound("No such user found!");

            if (string.IsNullOrEmpty(editUserProfileModel.UserName) || string.IsNullOrEmpty(editUserProfileModel.Email))
            {
                user.UserName = user.UserName;
                user.Email = user.Email;
            }
            else
            {
                user.UserName = editUserProfileModel.UserName;
                user.Email = editUserProfileModel.Email;
            }

            user.FirstName = editUserProfileModel.FirstName;
            user.LastName = editUserProfileModel.LastName;
            user.PhoneNumber = editUserProfileModel.PhoneNumber;
            user.AcceptTerms = user.AcceptTerms;

            var result = await _service.EditUserProfileAsync(user).ConfigureAwait(false);

            if (result.Succeeded)
                return Ok("User profile has been updated");
            else
                return BadRequest(result.Errors);
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


