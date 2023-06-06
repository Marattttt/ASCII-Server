using System.Text;
using System.Text.Json;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;

using shared.Config;

namespace api.Services;

public class ImagesService
{
    IDataProtector _protector;
    FilePathService _filePathService;

    const long MaxFileSizeBytes = 5 * 1_000_000_0; //5 megabytes
    public ImagesService(IDataProtectionProvider dataProtectionProvider) {
        _protector = dataProtectionProvider.CreateProtector(FileConfig.FileNameProtectionSeed);
        _filePathService = new FilePathService(_protector);
    }

    public Stream? GetFile(string fileName, string sender) {
        string protectedFileName;
        string fileDirectory;
        _filePathService.GetFilePath(fileName, sender, out protectedFileName, out fileDirectory);
        string fullPath = Path.Combine(fileDirectory, protectedFileName);
        if (!File.Exists(fullPath)){
            return null;
        }

        FileStream fs = new FileStream(fullPath, FileMode.Open);

        return fs;
    }

    //Method returns path to the saved file, rewrites files with the same name and sender 
    //and returns null if faces an error
    public async Task<string?> SaveFileAsync(IFormFile file, string sender) {
        if (!isFileValid(file)) {
            return null;
        }
        string protectedFileName;
        string senderOutputDirectory;
        _filePathService.CreateFilePath(file, sender, out protectedFileName, out senderOutputDirectory);
        string outPath = Path.Combine(senderOutputDirectory, protectedFileName);
        using (var stream = File.Create(outPath)) {
            await file.CopyToAsync(stream);
        }
        return outPath;
    }

    private bool isFileValid(IFormFile file) {
        if (file.Length <= 0 || file.Length > MaxFileSizeBytes) {
            return false;
        }
        string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension)) {
            return false;
        }
        return true;
    }
}