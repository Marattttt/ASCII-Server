using shared.DTOs;

namespace api.Services.Processing;

public interface IImageProcessor {
    public Task ProcessImageAsync(ImageDataDTO dto, int newWidth = 0, int newHeight = 0);
}