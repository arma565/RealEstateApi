using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly RepositoryService _service;
    private readonly PasswordHelper _passwordHelper;
    private readonly HttpClient _httpClient;

    public AuthController(
        RepositoryService service,
        PasswordHelper passwordHelper,
        HttpClient httpClient
    )
    {
        _service = service;
        _passwordHelper = passwordHelper;
        _httpClient = httpClient;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        return Ok(await _service.GetAllUsers());
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] Register model)
    {
        if (model.Password != model.RepeatPassword)
        {
            return BadRequest("Passwords do not match!");
        }
        if (model.AcceptTerms == false)
        {
            return BadRequest("Please accept our terms!");
        }
        var res = await _service.RegisterUser(model);
        if (res.Succeeded)
        {
            return Ok("User created successfully");
        }
        return BadRequest(res.Errors);
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllUsers()
    {
        await _service.DeleteAllUsers();
        var users = await _service.GetAllUsers();
        if (users.IsNullOrEmpty())
        {
            return Ok("All users deleted");
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
            return Ok("User deleted successfully");
        }
        return BadRequest(res.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] Login model)
    {
        var result = await _service.LoginUser(model);
        if (result.Succeeded)
        {
            return Ok("Login successful");
        }
        return Unauthorized("Username or password is not correct! please try again");
    }

    [HttpGet("recovery/{userEmail}")]
    public async Task<IActionResult> RecoverUser(string userEmail = "")
    {
        var user = await _service.FindUserByEmail(userEmail);
        if (user == null)
        {
            return BadRequest("No such user found!");
        }

        var generatedToken = await _service.GenerateTokenToRecoverUser(user);
        return Ok(generatedToken);
    }

    [HttpPost("reset/password")]
    public async Task<IActionResult> ResetPassword(Reset model)
    {
        var user = await _service.FindUserByEmail(model.Email);
        if (user == null)
        {
            return NotFound("No such user found!");
        }
        if (model.NewPassword != model.RepeatNewPassword)
        {
            return BadRequest("Passwords do not match!");
        }

        var result = await _service.ResetPassword(user, model.Token, model.NewPassword);
        if (result.Succeeded)
        {
            return Ok("Password reset was successful");
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("Image not provided!");
        }
        await _service.UploadProfileImage(image);
        return Ok(
            Url.Action(
                action: "DownloadProfileImage",
                controller: "Auth",
                values: new { image.FileName },
                protocol: Request.Scheme
            )
        );
    }

    [HttpGet("download/{fileName}")]
    public IActionResult DownloadProfileImage(string fileName)
    {
        //check if filename is not empty or null
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("File name can't be empty!");
        }
        if (fileName.Contains("..") || Path.GetInvalidFileNameChars().Any(fileName.Contains))
        {
            return BadRequest("Invalid file name.");
        }

        //check directory if exist
        var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        var filePath = Path.Combine(imageDir, fileName);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File not found");
        }
        // Get the file's content type
        var fileExtension = Path.GetExtension(fileName).ToLower();
        var contentType = fileExtension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream",
        };

        try
        {
            // Open the file stream asynchronously
            FileStream stream = _service.ReadProfileImage(filePath);
            // Return the file stream as FileContentResult
            return File(stream, contentType, fileName);
        }
        catch (IOException ex)
        {
            // Log the error if needed
            return StatusCode(500, "Error reading the file!. Error =" + ex.Message);
        }
    }

    [HttpGet("getFromUrl")]
    public async Task<IActionResult> GetImageFromUrl([FromQuery] string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
        {
            return BadRequest("URL parameter is required.");
        }

        try
        {
            // Fetch the image from the URL
            var response = await _httpClient.GetAsync(fileUrl);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to fetch the image.");
            }

            // Get the image content as a stream
            var imageStream = await response.Content.ReadAsStreamAsync();
            var contentType =
                response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

            // Return the image stream
            return File(imageStream, contentType);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, $"Error fetching the image: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpPut("edit/profile")]
    public async Task<IActionResult> EditUserProfile([FromBody] Profile model)
    {
        var user = await _service.FindUserByUserName(model.UserName);
        if (user == null)
        {
            return NotFound("No such user found!");
        }

        user.ProfileImagePath = model.ProfileImagePath;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        var result = await _service.EditUserProfile(user);
        if (result.Succeeded)
        {
            return Ok("Profile updated successfully");
        }
        return BadRequest(result.Errors);
    }
}
