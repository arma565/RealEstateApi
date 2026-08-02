using RealEstate.Services.Models.Images;

namespace RealEstate.Services.Images;

#pragma warning disable CA1515
public sealed class ImageService(
    IWebHostEnvironment environment
    )
{
    private readonly IWebHostEnvironment _environment = environment;

    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public async Task<List<string>> SaveImages(IFormFile[] images, string path)
    {
        var fileNameList = new List<string>();

        ArgumentNullException.ThrowIfNull(images);

        foreach (var image in images)
        {
            if (!IsValidImage(image))
                throw new InvalidOperationException("Invalid image file.");
        }

        var saveFolderPath = Path.Combine(_environment.WebRootPath, path);

        if (!Directory.Exists(saveFolderPath))
            Directory.CreateDirectory(saveFolderPath);

        foreach (var image in images)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(saveFolderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream).ConfigureAwait(false);
            }
            fileNameList.Add(fileName);
        }

        return fileNameList;
    }

    public async Task DeleteImages(IEnumerable<RealEstateImage> Images)
    {
        if(Images == null)
            ArgumentNullException.ThrowIfNull(Images);

        var environmentPath = _environment.WebRootPath;

        foreach (var img in Images)
        {
            string imageFileName = $"{img.ImageFileUrl}";
            string thumbnailFileName = $"{img.ThumbnailFileUrl}";
            string fileName;
            if (!string.IsNullOrEmpty(imageFileName))
                fileName = imageFileName;
            else if (string.IsNullOrEmpty(thumbnailFileName))
                fileName = thumbnailFileName;
            else
                fileName = "";
            string filePath = Path.Combine(environmentPath, fileName);
            if (Directory.Exists(filePath))
                File.Delete(filePath);
            else
                continue;
        }
    }

    public string GetImagePath(string modelName)
    {
        var webRootPath = _environment.WebRootPath;

        return modelName switch
        {
            "property" => Path.Combine(webRootPath, "images\\property"),
            "user" => Path.Combine(webRootPath, "images\\user"),
            "support" => Path.Combine(webRootPath, "images\\support"),
            "" => Path.Combine(webRootPath, "images"),
            _ => Path.Combine(webRootPath)
        };
    }

    private static bool IsValidImage(IFormFile image)
    {
        if (image.Length == 0)
            return false;

        if (image.Length > MaxFileSize)
            return false; // File is too large

        var validExtensions = new[] { ".JPG", ".JPEG", ".PNG", ".GIF" };

        var extension = Path.GetExtension(image.FileName)?.ToUpperInvariant();

        if (!validExtensions.Contains(extension))
            return false; // Invalid file type

        return true;
    }


}

