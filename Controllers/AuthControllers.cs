using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

[ApiController]
[Route("[controller]")]
public class AuthControllers : ControllerBase
{
    private readonly RepositoryService _service;
    public AuthControllers(RepositoryService service)
    {
        _service = service;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserProfileIdentity>>> GetAllUsers()
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
            return Ok(new { Message = "User created successfully" });
        }
        return BadRequest(res.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] Login model)
    {
        var result = await _service.LoginUser(model);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Login successful" });
        }
        return Unauthorized(
            new { Message = "Username or password is not correct! please try again" }
        );
    }

    [HttpGet("recovery/{userEmail}")]
    public async Task<IActionResult> RecoverUser(string userEmail = "")
    {
        var user = await _service.FindUserByEmail(userEmail);
        if (user == null)
        {
            return Ok(new { Message = "If the email exists, a password reset link will be sent." });
        }

        var generatedToken = await _service.GenerateTokenToRecoverUser(user);
        var resetLink = Url.Action(
            "ResetUser",
            "AuthControllers",
            new { email = userEmail, token = generatedToken },
            Request.Scheme
        );

        // Here, you'd send the reset link via email
        // For example, use an email service like SendGrid, SMTP, etc.
        Console.WriteLine("Your reset link is:" + resetLink);

        return Ok(
            new
            {
                Message = "if the email exists, a password reset link will be sent. " + resetLink,
            }
        );
    }

    [HttpGet("hidden-route")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> ResetUser(string email, string token)
    {
        var user = await _service.FindUserByEmail(email);
        if (user == null || string.IsNullOrEmpty(token))
        {
            return BadRequest(new { Message = "Invalid password reset link!" });
        }
        var newUser = new Reset() { Email = email, Token = token };
        return Ok(newUser);
    }

    [HttpPost("reset/password")]
    public async Task<IActionResult> ResetPassword(Reset model)
    {
        var user = await _service.FindUserByEmail(model.Email);
        if (user == null)
        {
            return NotFound(new { Message = "No such user found!" });
        }
        if (model.NewPassword != model.RepeatNewPassword)
        {
            return BadRequest("Passwords do not match!");
        }

        var result = await _service.ResetPassword(user, model.Token, model.NewPassword);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Password reset was successful" });
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest(new { Message = "Image not provided" });
        }
        
        var filePath = await _service.UploadProfileImage(image);
        return Ok(new { Message = "Image saved successfully filePath: " + filePath });
    }

    [HttpGet("download/{fileName}")]
    public IActionResult DownloadProfileImage(string fileName)
    {
        //check if filename is not empty or null
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("File name can't be empty!");
        }

        //check directory if exist
        var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        var filePath = Path.Combine(imageDir, fileName);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File not found");
        }
         // Get the file's content type
        var contentType = "application/octet-stream";
        var fileExtension = Path.GetExtension(fileName).ToLower();
        if (fileExtension == ".jpg" || fileExtension == ".jpeg") contentType = "image/jpeg";
        else if (fileExtension == ".png") contentType = "image/png";

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
            return StatusCode(500, new { Message = "Error reading the file.", Error = ex.Message });
        }
    }

    [HttpPut("profile")]
    public async Task<IActionResult> EditUserProfile([FromBody] Profile model)
    {
        var user = await _service.FindUserByUserName(model.UserName);
        if (user == null)
        {
            return NotFound(new { Message = "No such user found" });
        }

        user.ProfileImagePath = model.ProfileImagePath;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        var result = await _service.EditUserProfile(user);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Profile updated successfully" });
        }
        return BadRequest(result.Errors);
    }
}
