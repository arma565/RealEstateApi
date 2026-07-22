using Microsoft.IdentityModel.Tokens;

#pragma warning disable CA1515
namespace RealEstate.Services.Images;

public sealed class ImageService(
    IWebHostEnvironment environment
    )
{
    private readonly IWebHostEnvironment _environment = environment;

    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    /// <summary>
    /// Use this function to upload profile image to server
    /// </summary>
    /// <param name="image">
    /// image to upload
    /// </param>
    /// <returns></returns>
   public async Task<string> UploadProfileImage(IFormFile image)
    {
        if (image is null)
            return "";

        if (!IsValidImage(image))
            throw new InvalidOperationException("Invalid image file.");

        var webRootPath = _environment.WebRootPath;

        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);

        if (!Directory.Exists(webRootPath))
            Directory.CreateDirectory(webRootPath); // Recreate wwwroot

        var uploadsFolder = Path.Combine(webRootPath, "images/auth");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream).ConfigureAwait(false);
        }

        return fileName;
    }

    /// <summary>
    /// Use this function to upload list of images to server
    /// </summary>
    /// <param name="images">
    /// list of images to upload
    /// </param>
    /// <returns>
    /// list of images as string
    /// </returns>
    public async Task<List<string>> UploadImages(IFormFile[] images)
    {
        var fileNameList = new List<string>();

        if (images == null || images.Length == 0)
            return fileNameList;

        foreach (var image in images)
        {
            if (!IsValidImage(image))
                throw new InvalidOperationException("Invalid image file.");
        }

        var webRootPath = _environment.WebRootPath;

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images/asset");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        foreach (var image in images)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream).ConfigureAwait(false);
            }
            fileNameList.Add(fileName);
        }

        return fileNameList;
    }

   public async Task<string> UploadSupportImage(IFormFile image)
    {
        if (image is null)
            return "";

        if (!IsValidImage(image))
            throw new InvalidOperationException("Invalid image file.");

        var webRootPath = _environment.WebRootPath;

        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);

        if (!Directory.Exists(webRootPath))
            Directory.CreateDirectory(webRootPath); // Recreate wwwroot

        var uploadsFolder = Path.Combine(webRootPath, "images/support");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream).ConfigureAwait(false);
        }

        return fileName;
    }

    private static bool IsValidImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
            return false;

        if (image.Length > MaxFileSize)
            return false; // File is too large

        var validExtensions = new[] { ".JPG", ".JPEG", ".PNG", ".GIF" };

        var extension = Path.GetExtension(image.FileName)?.ToUpperInvariant();

        if (!validExtensions.Contains(extension))
            return false; // Invalid file type

        return true;
    }

    public string GetLocalImagesFullPath(string requestedModelPath)
    {
        if (string.IsNullOrEmpty(requestedModelPath))
            return "";

        var webRootPath = _environment.WebRootPath;

        if (!Directory.Exists(webRootPath))
            return "";

        return requestedModelPath switch
        {
            "asset" => Path.Combine(webRootPath, "images\\asset"),
            "auth" => Path.Combine(webRootPath, "images\\auth"),
            "support" => Path.Combine(webRootPath, "images\\support"),
            _ => Path.Combine(webRootPath, "images"),
        };
    }
}

