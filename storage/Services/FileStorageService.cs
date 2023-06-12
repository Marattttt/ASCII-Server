using System.IO;

using shared.Config;
namespace storage.Services;

public static class FileStorage {
    public static async Task<string> SaveToTempFilePath(byte[] data) {
        if (!Directory.Exists(FileConfig.TempDirectory)) {
            Directory.CreateDirectory(FileConfig.TempDirectory);
        }
        
        string path = Path.Combine(
            FileConfig.TempDirectory, 
            Path.GetRandomFileName());

        while (File.Exists(path)) {
            path = Path.Combine(
                FileConfig.TempDirectory, 
                Path.GetRandomFileName());
        }

        using (var fs = new FileStream(path, FileMode.Append, FileAccess.Write)) {
            await fs.WriteAsync(data, 0, data.Count());
        }
        return path;
    }
}
