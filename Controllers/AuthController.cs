using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly RepositoryService _service;
    private readonly PasswordHelper _passwordHelper;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        RepositoryService service,
        PasswordHelper passwordHelper,
        ILogger<AuthController> logger
    )
    {
        _service = service;
        _passwordHelper = passwordHelper;
        _logger = logger;
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
        var imageDir = Path.Combine("wwwroot", "images");
        var filePath = Path.Combine(
            imageDir,
            fileName.Trim().Replace(" ", "").Replace("-", "").Replace("_", "")
        );
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

    [HttpPut("edit/profile/{userName}")]
    public async Task<IActionResult> EditUserProfile(
        IFormFile image,
        string userName,
        [FromForm] Profile model
    )
    {
        var user = await _service.FindUserByUserName(userName);
        if (user == null)
        {
            return NotFound("No such user found!");
        }
        if (image == null || image.Length == 0)
        {
            return BadRequest("Image not provided!");
        }
        var fileName = await _service.UploadProfileImage(image);
        var imageUrl = Url.Action(
            action: "DownloadProfileImage",
            controller: "Auth",
            values: new { fileName },
            protocol: Request.Scheme
        );
        user.ProfileImageUrl = imageUrl!;
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
