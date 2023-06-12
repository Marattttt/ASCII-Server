using shared.DTOs;

namespace api.Services;

public interface IUploadsManager {
    public Task<string?> UploadImageAsync (ImageDataDTO image);
}