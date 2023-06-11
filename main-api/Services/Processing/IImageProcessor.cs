using shared.DTOs;

namespace api.Services;

public interface IImageProcessor {
    public Task<string?> ProcessImageAsync(ImageToAsciiDTO dto); 
}