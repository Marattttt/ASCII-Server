using shared.DTOs;

namespace api.Services;

public interface IUploadsManager {
    public Task<string?> UploadImage (FullUserInfoDTO user, ImageDataDTO image);
}