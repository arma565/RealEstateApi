using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using RealEstate.Helper;
using RealEstate.Models.Authentication;
using RealEstate.Models.Estate.Assets;
using RealEstate.Services;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

#pragma warning disable CA1515
#pragma warning disable CA3003
namespace RealEstate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class AuthController(
        RepositoryService service,
        ImageService imageService,
        PasswordHelper passwordHelper
        ) : ControllerBase
    {
        private readonly RepositoryService _service = service;
        private readonly ImageService _imageService = imageService;
        private readonly PasswordHelper _passwordHelper = passwordHelper;


        [HttpPost("user/upload/{userID}")]
        public async Task<IActionResult> UploadProfileImage(string userID, IFormFile image)
        {
            try
            {
                if (image == null)
                    return BadRequest("Image can not be empty!");

                var imageFileName = await _imageService.UploadProfileImage(image).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(userID))
                    return BadRequest("userID can not be empty!");

                var user = await _service.FindUserByID(userID).ConfigureAwait(false);

                if (user == null)
                    return NotFound("No such user found!");

                user.ProfileImageName = imageFileName;

                var result = await _service.EditUserProfile(user).ConfigureAwait(false);
                if (result.Succeeded)
                    return Ok("ProfileImage successfully uploaded");
                else
                    return BadRequest(result.Errors);
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }

        [HttpGet("user/download/{userID}")]
        public async Task<IActionResult> DownloadProfileImage(string userID)
        {
            try
            {
                if (string.IsNullOrEmpty(userID))
                    return BadRequest("User id can not be empty!");

                var user = await _service.FindUserByID(userID).ConfigureAwait(false);

                if (user == null)
                    return NotFound("No such user found!");


                var profileImg = user.ProfileImageName;

                if (profileImg.IsNullOrEmpty())
                    return NotFound("No image found!");

                if (!Regex.IsMatch(Path.GetFileNameWithoutExtension(profileImg), @"^[a-zA-Z0-9_-]+$"))
                    return BadRequest("Invalid file name format.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(profileImg);
                if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                    return BadRequest("Unsupported image file type.");


                var environmentPath = _imageService.GetLocalImagesFullPath("auth");
                var combinedPath = Path.Combine(environmentPath, profileImg);
                var normalizedPath = Path.GetFullPath(combinedPath);

                if (!normalizedPath.StartsWith(environmentPath, StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Invalid file path.");

                if (!System.IO.File.Exists(normalizedPath))
                    return NotFound("Image file not found!");

                var provider = new FileExtensionContentTypeProvider();

                var contentType = provider.TryGetContentType(normalizedPath, out var type) ? type : "application/octet-stream";

                return PhysicalFile(normalizedPath, contentType, Path.GetFileName(normalizedPath));
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<User>>> GetAllUsers() => Ok(await _service.GetAllUsers().ConfigureAwait(false));

        [HttpGet("user/{userID}")]
        public async Task<ActionResult<User>> GetUserByUserName(string userID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userID))
                    return BadRequest("User id can not be empty!");

                var user = await _service.FindUserByID(userID).ConfigureAwait(false);

                if (user == null)
                    return NotFound("No such user found!");

                return Ok(user);

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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }

        [HttpPost("user/register")]
        public async Task<IActionResult> RegisterUser([FromBody] Register registerUser)
        {
            try
            {
                var allUsers = await _service.GetAllUsers().ConfigureAwait(false);

                if (allUsers.Any(u => u.UserName == registerUser.UserName))
                    return BadRequest("Username is already taken!");

                if (allUsers.Any(u => u.Email == registerUser.Email))
                    return BadRequest("Email is already taken!");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var res = await _service.RegisterUser(registerUser).ConfigureAwait(false);

                if (res.Succeeded)
                    return Ok("User registered successfully");

                return BadRequest(res.Errors);
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }

        [HttpPost("user/login")]
        public async Task<IActionResult> LoginUser([FromBody] Login model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.LoginUser(model).ConfigureAwait(false);

                if (result.Succeeded)
                    return Ok("Login successful");

                return Unauthorized("Username or password is not correct! please try again");
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }

        [HttpDelete("user/delete-all")]
        public async Task<IActionResult> DeleteAllUsers()
        {
            try
            {
                await _service.DeleteAllUsers().ConfigureAwait(false);
                return Ok("Users has been deleted");
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }

        [HttpDelete("user/delete/{userName}/{password}")]
        public async Task<IActionResult> DeleteUser(string userName, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                    return BadRequest("userName or password is empty!");

                var users = await _service.GetAllUsers().ConfigureAwait(false);

                var user = users.FirstOrDefault(user => user.UserName == userName);

                if (user == null)
                    return NotFound("No such user found!");

                if (!_passwordHelper.VerifyPassword(user, user.PasswordHash!, password))
                    return BadRequest("Password is not correct!");

                var res = await _service.DeleteUser(user).ConfigureAwait(false);

                if (res.Succeeded)
                    return NoContent();

                return BadRequest(res.Errors);
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }

        [HttpPost("user/recovery/account")]
        public async Task<ActionResult<string>> RecoverUser([FromBody] Recovery recovery)
        {
            try
            {
                if (recovery == null)
                    return BadRequest("Failed to retreive parameter!");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _service.FindUserByEmail(recovery.Email).ConfigureAwait(false);

                if (user == null)
                    return BadRequest("No such user found!");

                var generatedToken = await _service.GenerateTokenToRecoverUser(user).ConfigureAwait(false);

                return Ok(generatedToken);
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }

        [HttpPost("user/reset/password")]
        public async Task<IActionResult> ResetPassword([FromBody] Reset model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Failed to retreive parameter!");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _service.FindUserByEmail(model.Email).ConfigureAwait(false);

                if (user == null)
                    return NotFound("No such user found!");

                var result = await _service.ResetPassword(user, model.Token, model.NewPassword).ConfigureAwait(false);

                if (result.Succeeded)
                    return Ok("Password reset was successful");

                return BadRequest(result.Errors);
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }

        [HttpPost("user/change/password")]
        public async Task<IActionResult> ChangePassword([FromBody] Change model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Failed to retreive parameter!");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _service.FindUserByUserName(model.UserName).ConfigureAwait(false);

                if (user == null)
                    return NotFound("No such user found!");

                var result = await _service.ChangePassword(user, model.CurrentPassword, model.NewPassword).ConfigureAwait(false);

                if (result.Succeeded)
                    return Ok("Password has been changed");

                return BadRequest(result.Errors);
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }

        [HttpPut("user/edit/profile")]
        public async Task<IActionResult> EditUserProfile([FromBody] User updateUser)
        {
            try
            {
                if (updateUser == null)
                    return BadRequest("User ID is required!");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _service.FindUserByID(updateUser.Id).ConfigureAwait(false);

                if (user == null)
                    return NotFound("No such user found!");

                user.UserName = updateUser.UserName;
                user.Email = updateUser.Email;
                user.PhoneNumber = updateUser.PhoneNumber;
                user.FirstName = updateUser.FirstName;
                user.LastName = updateUser.LastName;
                user.AcceptTerms = user.AcceptTerms;
                user.ProfileImageName = user.ProfileImageName;

                var result = await _service.EditUserProfile(user).ConfigureAwait(false);

                if (result.Succeeded)
                    return Ok("User profile has been updated");
                else
                    return BadRequest(result.Errors);
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(403, "Access denied.");
            }
        }
    }
}

