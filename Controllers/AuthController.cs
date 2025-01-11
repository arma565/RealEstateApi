using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly RepositoryService _service;
    private readonly ImageService _imageService;
    private readonly PasswordHelper _passwordHelper;
    private readonly ILogger<AuthController> _logger;
    private readonly HttpClient _httpClient;

    public AuthController(
        RepositoryService service,
        ImageService imageService,
        PasswordHelper passwordHelper,
        ILogger<AuthController> logger,
        HttpClient httpClient
    )
    {
        _service = service;
        _imageService = imageService;
        _passwordHelper = passwordHelper;
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        return Ok(await _service.GetAllUsers());
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<User>> GetUserByUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
        {
            return BadRequest("Username can not be empty!");
        }
        var user = await _service.FindUserByUserName(userName);
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var res = await _service.RegisterUser(model);
        if (res.Succeeded)
        {
            return CreatedAtAction(nameof(GetUserByUserName), new { model.UserName }, model);
        }
        return BadRequest(res.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] Login model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _service.LoginUser(model);
        if (result.Succeeded)
        {
            return Ok("Login successful");
        }
        return Unauthorized("Username or password is not correct! please try again");
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllUsers()
    {
        await _service.DeleteAllUsers();
        var users = await _service.GetAllUsers();
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
        var user = await _service.FindUserByUserName(userName);
        if (user == null)
        {
            return BadRequest("No such user found!");
        }
        if (!_passwordHelper.VerifyPassword(user, user.PasswordHash!, password))
        {
            return BadRequest("Password is not correct!");
        }
        var res = await _service.DeleteUser(user);
        if (res.Succeeded)
        {
            return NoContent();
        }
        return BadRequest(res.Errors);
    }

    [HttpPost("recovery/account")]
    public async Task<ActionResult<string>> RecoverUser([FromBody] Recovery recovery)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = await _service.FindUserByEmail(recovery.Email);
        if (user == null)
        {
            return BadRequest("No such user found!");
        }

        var generatedToken = await _service.GenerateTokenToRecoverUser(user);
        return Ok(generatedToken);
    }

    [HttpPost("reset/password")]
    public async Task<IActionResult> ResetPassword([FromBody] Reset model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = await _service.FindUserByEmail(model.Email);
        if (user == null)
        {
            return NotFound("No such user found!");
        }
        var result = await _service.ResetPassword(user, model.Token, model.NewPassword);
        if (result.Succeeded)
        {
            return Ok("Password reset was successful");
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("change/password")]
    public async Task<IActionResult> ChangePassword([FromBody] Change model){
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }

        var user = await _service.FindUserByUserName(model.UserName);
        if (user == null)
        {
            return NotFound("No such user found!");
        }
          var result = await _service.ChangePassword(user, model.CurrentPassword, model.NewPassword);
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
            var user = await _service.FindUserByUserName(userName);
            if (user == null)
            {
                return NotFound("No such user found!");
            }
            if (
                string.IsNullOrEmpty(user.ProfileImageUrl)
                || !Uri.IsWellFormedUriString(user.ProfileImageUrl, UriKind.Absolute)
            )
            {
                return BadRequest("Invalid URL");
            }
            var response = await _httpClient.GetAsync(user.ProfileImageUrl);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to download image");
            }
            // Read the image as a byte array
            var imageData = await response.Content.ReadAsByteArrayAsync();

            // Return the image as a file response
            var contentType =
                response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            return Ok(File(imageData, contentType));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpPost("upload/{userName}")]
    public async Task<IActionResult> UploadProfileImage(string userName, IFormFile image)
    {
        try
        {
            var user = await _service.FindUserByUserName(userName);
            if (user == null)
            {
                return NotFound("No such user found!");
            }
            var imageUrl = await _imageService.UploadProfileImage(image);
            user.ProfileImageUrl = imageUrl!;
            var result = await _service.EditUserProfile(user);
            if (result.Succeeded)
            {
                return Ok(imageUrl);
            }
            return BadRequest(result.Errors);
        }
        catch (IOException ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while editing profile for user: {UserName}",
                userName
            );
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while editing profile for user: {UserName}",
                userName
            );
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpPut("edit/profile")]
    public async Task<IActionResult> EditUserProfile([FromBody] Profile model)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var user = await _service.FindUserByUserName(model.UserName);
        if (user == null)
        {
            return NotFound("No such user found!");
        }
        user.UserName = model.UserName;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        var result = await _service.EditUserProfile(user);
        if (result.Succeeded)
        {
            return NoContent();
        }
        return BadRequest(result.Errors);
    }
}
