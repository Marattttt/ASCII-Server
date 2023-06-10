using shared.DTOs;

namespace api.Services;

public interface IProcessor {
    public Task<string?> ProcessImageAsync(ImageToAsciiDTO dto); 
}