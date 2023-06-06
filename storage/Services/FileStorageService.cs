using System.IO;

using shared.Config;
using storage.Models;

namespace storage.Services;

public static class FileStorageService {
    public static string GetTempFilePath() {
        if (!Directory.Exists(FileConfig.TempDirectory)) {
            Directory.CreateDirectory(FileConfig.TempDirectory);
        }

        string randomFilePath = Path.Combine(
            FileConfig.TempDirectory, 
            Path.GetRandomFileName());
        Console.WriteLine((randomFilePath));
        
        return randomFilePath;
    }
    public static async Task<string> SaveToTempFilePath(byte[] data) {
        string path = GetTempFilePath();
        using (var fs = new FileStream(path, FileMode.Append, FileAccess.Write)) {
            await fs.WriteAsync(data, 0, data.Count());
        }
        return path;
    }
}
