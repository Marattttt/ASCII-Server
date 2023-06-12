using shared.Config;

namespace api.Services;

public class LocalFilesManager : IFilesManager {
    public async Task<string> SaveDataAsync(byte[] data, string extension) { 
        string newPath = Path.Combine(
            FileConfig.TempDirectory,
            Path.GetRandomFileName()
        );
        while (File.Exists(newPath)) {
            newPath = Path.Combine(
                    FileConfig.TempDirectory,
                    Path.GetRandomFileName()
            );
        }
        using (var fs = new FileStream(newPath, FileMode.OpenOrCreate, FileAccess.Write)) {
            await fs.WriteAsync(data, 0, data.Length);
        }
        return newPath;
    }
}