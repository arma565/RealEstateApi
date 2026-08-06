using SkiaSharp;

namespace RealEstate.Images;

interface IImageService
{
    Task<ImagePaths> SaveAsync(IFormFile image);

    Task<DownloadPaths> GetPathsAsync(ImagePaths paths);

    Task DeleteFilesAsync(ImagePaths paths);
}

#pragma warning disable CA1515
#pragma warning disable CS9124
public class ImageService(IWebHostEnvironment environment) : IImageService
{
    private readonly IWebHostEnvironment _environment = environment;

    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public async Task<ImagePaths> SaveAsync(IFormFile image)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (!IsValidImage(image))
            throw new InvalidOperationException("Invalid image file.");

        var originalFileName = await CreateImages(image, _environment).ConfigureAwait(false);

        string relativeFolderPath = Path.Combine("RealEstate", "Uploads");

        var imagePaths = new ImagePaths
        {
            OriginalPath = Path.Combine(relativeFolderPath, "Images", originalFileName),
            ThumbnailPath = Path.Combine(relativeFolderPath, "Thumbnails", originalFileName)
        };

        return imagePaths;
    }

    public async Task<DownloadPaths> GetPathsAsync(ImagePaths paths)
    {
        if (paths == null)
            ArgumentNullException.ThrowIfNull(paths);
        if (!File.Exists(paths.OriginalPath) || !File.Exists(paths.ThumbnailPath))
            throw new FileNotFoundException("One or both image files not found.");

        return new DownloadPaths
        {
            FullOriginalPath = Path.Combine(environment.ContentRootPath, paths.OriginalPath),
            FullThumbnailPath = Path.Combine(environment.ContentRootPath, paths.ThumbnailPath)
        };
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

        string originalFolderPath = Path.Combine(_environment.ContentRootPath, "RealEstate", "Uploads", "Images");

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

        string thumbnailFolderPath = Path.Combine(_environment.ContentRootPath, "RealEstate", "Uploads", "Thumbnails");

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
    public string? OriginalPath { get; set; } = null!;
    public string? ThumbnailPath { get; set; } = null!;
}

public record DownloadPaths
{
    public string? FullOriginalPath { get; set; } = null!;
    public string? FullThumbnailPath { get; set; } = null!;
}


