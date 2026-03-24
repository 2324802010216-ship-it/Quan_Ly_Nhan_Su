namespace WebQuanLyNhanSu.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public FileUploadService(IWebHostEnvironment env, IConfiguration config)
    { _env = env; _config = config; }

    public async Task<string?> UploadFile(IFormFile file, string subFolder)
    {
        if (file == null || file.Length == 0) return null;

        var maxSize = _config.GetValue<long>("FileUpload:MaxFileSize", 5242880);
        if (file.Length > maxSize) throw new Exception("File quá lớn (tối đa 5MB)");

        var allowed = _config.GetValue<string>("FileUpload:AllowedExtensions", ".jpg,.jpeg,.png,.pdf,.doc,.docx");
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed!.Split(',').Contains(ext))
            throw new Exception($"Định dạng file không hỗ trợ ({ext})");

        var uploadPath = Path.Combine(_env.WebRootPath, subFolder);
        Directory.CreateDirectory(uploadPath);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/{subFolder}/{fileName}";
    }

    public void DeleteFile(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return;
        var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
}
