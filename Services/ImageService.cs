using Microsoft.AspNetCore.Mvc;

public interface IImageService
{
    Task<string> UploadProfileImage(IFormFile image);
    FileStream ReadProfileImage(string filePath);
}

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImageService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public ImageService(
        IWebHostEnvironment environment,
        ILogger<ImageService> logger,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _environment = environment;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Use this function to upload profile image to server
    /// </summary>
    /// <param name="image">
    /// image to upload
    /// </param>
    /// <returns></returns>
    public async Task<string> UploadProfileImage(IFormFile image)
    {
        if (!IsValidImage(image))
        {
            throw new InvalidOperationException("Invalid image file.");
        }

        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }
        return GetFileUrl(fileName);
    }

    /// <summary>
    /// Use this to download image from server
    /// </summary>
    /// <param name="filePath">
    /// file path of image file
    /// </param>
    /// <returns></returns>
    /// <exception cref="IOException"></exception>
    public FileStream ReadProfileImage(string filePath)
    {
        try
        {
            return new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                useAsync: true
            );
        }
        catch (IOException ex)
        {
            throw new IOException("Error reading the file. Error =" + ex.Message);
        }
    }

    private bool IsValidImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return false;
        }

        if (image.Length > MaxFileSize)
        {
            return false; // File is too large
        }

        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(image.FileName)?.ToLowerInvariant();

        if (!validExtensions.Contains(extension))
        {
            return false; // Invalid file type
        }

        return true;
    }

    private string GetFileUrl(string fileName)
    {
        var connection = _httpContextAccessor.HttpContext?.Connection;
        if (connection != null)
        {
            var localIpAddress = connection.LocalIpAddress?.ToString();

            if (localIpAddress == "127.0.0.1" || localIpAddress == "::1")
            {
                return $"http://localhost:5068/images/{fileName}";
            }
            else
            {
                return $"http://{connection.RemoteIpAddress}:5068/images/{fileName}";
            }
        }
        else
        {
            return $"http://localhost:5068/images/{fileName}";
        }
    }
}
