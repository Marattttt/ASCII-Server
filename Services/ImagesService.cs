using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

using api.Configuration;

namespace api.Services;

public class ImagesService
{
    FileConfig _fileConfig;

    IWebHostEnvironment _environment;
    IDataProtector _protector;
    public ImagesService(IWebHostEnvironment environment, IDataProtectionProvider dataProtectionProvider)
    {
        _environment = environment;

        string configPath = Path.Combine(
            environment.ContentRootPath, 
            "Config/FileSettings.json");

        string json = File.ReadAllText(configPath);

        try {
            _fileConfig = JsonSerializer.Deserialize<FileConfig>(json)!;
        } 
        catch (Exception e)
        {
            Console.WriteLine($"Config file at {configPath} is not alligned with FileConfig class");
            Console.WriteLine("Exception message: ", e.Message);
            _fileConfig = new FileConfig();
        }
        _protector = dataProtectionProvider.CreateProtector(_fileConfig.FileNameProtectionSeed);
    }
    private bool isFileValid(IFormFile file)
    {
        if (file.Length <= 0)
            return false;

        string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            return false;

        return true;
    }

    public async Task<int> SaveFileAsync(IFormFile file, string sender)
    {
        if (!isFileValid(file))
            return 0;
        
        Console.WriteLine(_fileConfig.OutputDirectory);
        Console.WriteLine(_fileConfig.FileNameProtectionSeed);

        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        string fileNameToProtect = file.FileName.Remove(file.FileName.LastIndexOf('.')) + sender;

        string protectedFileName = _protector.Protect(fileNameToProtect);

        string outPath = Path.Combine(_fileConfig.OutputDirectory, protectedFileName + extension);
        if (File.Exists(outPath))
        using (var stream = File.Create(outPath))
        {
            await file.CopyToAsync(stream);
        }
        return 1;
    }


}