namespace api.Services;

public interface IFilesManager {
    public Task<string> SaveDataAsync(byte[] data, string extension);
}