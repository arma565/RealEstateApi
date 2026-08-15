using SkiaSharp;

namespace RealEstate.Services.Images;

interface IImageProccess
{
    Task<ImagePaths> SaveAsync(IFormFile image);

    Task<string> GetFullPathsAsync(string path);

    Task DeleteFilesAsync(ImagePaths paths);
}


#pragma warning disable CA1515
public class ImageProccess( IWebHostEnvironment environment) : IImageProccess
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public async Task<ImagePaths> SaveAsync(IFormFile image)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (!IsValidImage(image))
            throw new InvalidOperationException("Invalid image file.");

        var originalFileName = await CreateImages(image, environment).ConfigureAwait(false);

        string relativeFolderPath = Path.Combine("Uploads");

        var imagePaths = new ImagePaths
        {
            OriginalPath = Path.Combine(relativeFolderPath, "Images", originalFileName),
            ThumbnailPath = Path.Combine(relativeFolderPath, "Thumbnails", originalFileName)
        };

        return imagePaths;
    }

    public async Task<string> GetFullPathsAsync(string path)
    {
        if (path == null)
            ArgumentNullException.ThrowIfNull(path);

        return Path.Combine(environment.ContentRootPath, path);
    }

    public async Task DeleteFilesAsync(ImagePaths paths)
    {
        if (paths == null)
            ArgumentNullException.ThrowIfNull(paths);

        if (paths.OriginalPath == null)
            ArgumentNullException.ThrowIfNull(paths.OriginalPath);

        if (paths.ThumbnailPath == null)
            ArgumentNullException.ThrowIfNull(paths.ThumbnailPath);

        var downloadPaths = new DownloadPaths
        {
            FullOriginalPath = Path.Combine(environment.ContentRootPath, paths.OriginalPath),
            FullThumbnailPath = Path.Combine(environment.ContentRootPath, paths.ThumbnailPath)
        };

        if (File.Exists(downloadPaths.FullOriginalPath))
            File.Delete(downloadPaths.FullOriginalPath);
        if (File.Exists(downloadPaths.FullThumbnailPath))
            File.Delete(downloadPaths.FullThumbnailPath);
    }

    private static bool IsValidImage(IFormFile image)
    {
        var validExtensions = new[] { ".JPG", ".JPEG", ".PNG", ".GIF" };

        var extension = Path.GetExtension(image.FileName)?.ToUpperInvariant();

        if (image == null || image.Length == 0 || image.Length > MaxFileSize || !validExtensions.Contains(extension))
            return false;

        return true;
    }

    private static async Task<string> CreateImages(IFormFile image , IWebHostEnvironment _environment) {

        string originalFolderPath = Path.Combine(_environment.ContentRootPath, "Uploads", "Images");

        if (!Directory.Exists(originalFolderPath))
            Directory.CreateDirectory(originalFolderPath);

        var originalFileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
        var originalFilePath = Path.Combine(originalFolderPath, originalFileName);

        using (var stream = new FileStream(originalFilePath, FileMode.Create))
        {
            await image.CopyToAsync(stream).ConfigureAwait(false);
        }

        await CreateThumbnail(originalFileName, originalFilePath, _environment).ConfigureAwait(false);

        return originalFileName;

    }

    private static async Task CreateThumbnail(string originalFileName , string originalFilePath , IWebHostEnvironment _environment) {

        string thumbnailFolderPath = Path.Combine(_environment.ContentRootPath, "Uploads", "Thumbnails");

        if (!Directory.Exists(thumbnailFolderPath))
            Directory.CreateDirectory(thumbnailFolderPath);

        var thumbnailFilePath = Path.Combine(thumbnailFolderPath, originalFileName);
        using var input = File.OpenRead(originalFilePath);
        using var original = SKBitmap.Decode(input);

        var resizedInfo = new SKImageInfo(300, 300);

        using var resized = original.Resize(
            resizedInfo,
            SKSamplingOptions.Default);

        using var originalImage = SKImage.FromBitmap(resized);
        using var data = originalImage.Encode(SKEncodedImageFormat.Jpeg, 90);

        using var output = File.OpenWrite(thumbnailFilePath);
        data.SaveTo(output);

    }
}

public record ImagePaths
{
    public required string OriginalPath { get; set; }
    public required string ThumbnailPath { get; set; }
}

public record DownloadPaths
{
    public required string FullOriginalPath { get; set; }
    public required string FullThumbnailPath { get; set; }
}


