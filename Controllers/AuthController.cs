using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Helper;
using RealEstate.Models.Authentication;
using RealEstate.Services;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
namespace RealEstate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class AuthController(
        RepositoryService service,
        ImageService imageService,
        PasswordHelper passwordHelper,
        HttpClient httpClient
        ) : ControllerBase
    {
        private readonly RepositoryService _service = service;
        private readonly ImageService _imageService = imageService;
        private readonly PasswordHelper _passwordHelper = passwordHelper;
        private readonly HttpClient _httpClient = httpClient;

        [HttpGet("users")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            return Ok(await _service.GetAllUsers().ConfigureAwait(false));
        }

        [HttpGet("{userName}")]
        public async Task<ActionResult<User>> GetUserByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Username can not be empty!");
            }
            var user = await _service.FindUserByUserName(userName).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound("No such user found!");
            }
            return Ok(
                new User
                {
                    Id = user!.Id,
                    ProfileImagePath = user!.ProfileImageUrl!,
                    FirstName = user.FirstName!,
                    LastName = user.LastName!,
                    AcceptTerms = user.AcceptTerms,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Register model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Failed to retreive parameter!");
                }
                // Check for existing username or email
                var userWithSameUsername = await _service.GetAllUsers().ConfigureAwait(false);
                if (userWithSameUsername.Any(u => u.UserName == model.UserName))
                {
                    return BadRequest("Username is already taken!");
                }
                if (userWithSameUsername.Any(u => u.Email == model.Email))
                {
                    return BadRequest("Email is already taken!");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _service.RegisterUser(model).ConfigureAwait(false);
                if (res.Succeeded)
                {
                    return CreatedAtAction(nameof(GetUserByUserName), new { model.UserName }, model);
                }
                return BadRequest(res.Errors);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Failed to retreive parameter!");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] Login model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _service.LoginUser(model).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok("Login successful");
            }
            return Unauthorized("Username or password is not correct! please try again");
        }

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllUsers()
        {
            await _service.DeleteAllUsers().ConfigureAwait(false);
            var users = await _service.GetAllUsers().ConfigureAwait(false);
            if (users.IsNullOrEmpty())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("Could not delete users!");
            }
        }

        [HttpDelete("delete/{userName}/{password}")]
        public async Task<IActionResult> DeleteUser(string userName, string password)
        {
            var user = await _service.FindUserByUserName(userName).ConfigureAwait(false);
            if (user == null)
            {
                return BadRequest("No such user found!");
            }
            if (!_passwordHelper.VerifyPassword(user, user.PasswordHash!, password))
            {
                return BadRequest("Password is not correct!");
            }
            var res = await _service.DeleteUser(user).ConfigureAwait(false);
            if (res.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(res.Errors);
        }

        [HttpPost("recovery/account")]
        public async Task<ActionResult<string>> RecoverUser([FromBody] Recovery recovery)
        {
            try
            {
                if (recovery == null)
                {
                    return BadRequest("Failed to retreive parameter!");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = await _service.FindUserByEmail(recovery.Email).ConfigureAwait(false);
                if (user == null)
                {
                    return BadRequest("No such user found!");
                }

                var generatedToken = await _service.GenerateTokenToRecoverUser(user).ConfigureAwait(false);
                return Ok(generatedToken);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Failed to retreive parameter!");
            }

        }

        [HttpPost("reset/password")]
        public async Task<IActionResult> ResetPassword([FromBody] Reset model)
        {
            if (model == null)
            {
                return BadRequest("Failed to retreive parameter!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _service.FindUserByEmail(model.Email).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound("No such user found!");
            }
            var result = await _service.ResetPassword(user, model.Token, model.NewPassword).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok("Password reset was successful");
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("change/password")]
        public async Task<IActionResult> ChangePassword([FromBody] Change model)
        {
            if (model == null)
            {
                return BadRequest("Failed to retreive parameter!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _service.FindUserByUserName(model.UserName).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound("No such user found!");
            }
            var result = await _service.ChangePassword(user, model.CurrentPassword, model.NewPassword).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok("Password has been changed");
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("download/{userName}")]
        public async Task<IActionResult> DownloadProfileImage(string userName)
        {
            try
            {
                //check if filename is not empty or null
                var user = await _service.FindUserByUserName(userName).ConfigureAwait(false);
                if (user == null)
                {
                    return NotFound("No such user found!");
                }
                if (user.ProfileImageUrl == null)
                {
                    return BadRequest("No image found!");
                }
                if (!Uri.IsWellFormedUriString(user.ProfileImageUrl.ToString(), UriKind.Absolute))
                {
                    return BadRequest("Invalid URL");
                }
                HttpResponseMessage response = await _httpClient.GetAsync(new Uri(user.ProfileImageUrl.ToString()))
                    .ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Failed to download image");
                }
                // Read the image as a byte array
                var imageData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                // Return the image as a file response
                var contentType =
                    response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                return File(imageData, contentType);
            }
            catch (BadHttpRequestException ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("upload/{userName}")]
        public async Task<IActionResult> UploadProfileImage(string userName, IFormFile image)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    return BadRequest("Username can not be empty!");
                }
                var user = await _service.FindUserByUserName(userName).ConfigureAwait(false);
                if (user == null)
                {
                    return NotFound("No such user found!");
                }
                if (image == null)
                {
                    return BadRequest("Image can not be empty!");
                }
                var imageUrl = await _imageService.UploadProfileImage(image).ConfigureAwait(false);
                user.ProfileImageUrl = new Uri(imageUrl);
                var result = await _service.EditUserProfile(user).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return Ok(imageUrl);
                }
                return BadRequest(result.Errors);
            }
            catch (IOException ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again later." + "Error detailes: " + ex.Message);
            }
        }

        [HttpPut("edit/profile")]
        public async Task<IActionResult> EditUserProfile([FromBody] UserProfile model)
        {
            if (model is null)
            {
                return BadRequest("Failed to retreive parameter!");
            }
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var user = await _service.FindUserByUserName(model.UserName).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound("No such user found!");
            }
            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            var result = await _service.EditUserProfile(user).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }
    }
}

