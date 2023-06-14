using shared.DTOs;

namespace api.Services;

public interface IUploadsManager {
    public Task<string?> UploadImageAsync (ImageDataDTO image);
    public Task<(Stream? stream, string? errorMessage)> GetImageAsync(
        int userId, 
        string fileName);
}