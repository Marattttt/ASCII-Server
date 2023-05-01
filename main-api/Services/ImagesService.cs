using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

using api.Configuration;

namespace api.Services;

public class ImagesService
{
    IDataProtector _protector;

    const long MaxFileSizeBytes = 5 * 1_000_000; //5 megabytes
    public ImagesService(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(FileConfig.FileNameProtectionSeed);
    }

    //Method returns path to the saved file, rewrites files with the same name and sender 
    //and returns null if faces an error
    public async Task<string?> SaveFileAsync(IFormFile file, string sender)
    {
        if (!isFileValid(file))
            return null;

        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        string fileNameToProtect = file.FileName.Remove(file.FileName.LastIndexOf('.'));

        string protectedFileName = _protector.Protect(fileNameToProtect);
        string senderOutputDirectory = Path.Combine(FileConfig.OutputDirectory, sender);
        string outPath = Path.Combine(
            senderOutputDirectory,
            protectedFileName + extension);
        
        if (File.Exists(outPath))
            File.Delete(outPath);
        if (!Directory.Exists(senderOutputDirectory))
            Directory.CreateDirectory(senderOutputDirectory);
        
        using (var stream = File.Create(outPath))
        {
            await file.CopyToAsync(stream);
        }
        return outPath;
    }

    private bool isFileValid(IFormFile file)
    {
        if (file.Length <= 0 || file.Length > MaxFileSizeBytes)
            return false;

        string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            return false;

        return true;
    }
}