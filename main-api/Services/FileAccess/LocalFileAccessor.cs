using System.Text;
using Microsoft.AspNetCore.DataProtection;

using shared.Config;

namespace api.Services;

// Any work with file paths should be standardized through this service;
// The class should be used inside other public service classes and kept internal
public class LocalFileAccessor {
    IDataProtector _protector;

    public LocalFileAccessor(IDataProtectionProvider dataProtectionProvider) {
        _protector = _protector = dataProtectionProvider.CreateProtector(
            FileConfig.FileNameProtectionSeed);;
    }
    public Stream? GetFile(string fileName, string sender) {
        string protectedFileName;
        string fileDirectory;
        GetFilePath(fileName, sender, out protectedFileName, out fileDirectory);
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
        CreateFilePath(file, sender, out protectedFileName, out senderOutputDirectory);
        string outPath = Path.Combine(senderOutputDirectory, protectedFileName);
        using (var stream = File.Create(outPath)) {
            await file.CopyToAsync(stream);
        }
        return outPath;
    }
    private bool isFileValid(IFormFile file) {
        if (file.Length <= 0 || file.Length > FileConfig.MaxFileSizeBytes) {
            return false;
        }
        string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension)) {
            return false;
        }
        return true;
    }

    private void CreateFilePath (
        IFormFile file, 
        string sender, 
        out string protectedFileName, 
        out string senderOutputDirectory) {

        GetFilePath(file.FileName, sender, out protectedFileName, out senderOutputDirectory);
        string newFullPath = Path.Combine(senderOutputDirectory, protectedFileName);
        if (Path.Exists(newFullPath)) {
            File.Delete(newFullPath);
        }
        if (!Directory.Exists(senderOutputDirectory)) {
            Directory.CreateDirectory(senderOutputDirectory);
        }
    }
    private void GetFilePath (
        string fileName, 
        string sender, 
        out string protectedFileName, 
        out string senderOutputDirectory) {

        string extension = Path.GetExtension(fileName).ToLowerInvariant();
        string fileNameToProtect = Path.GetFileNameWithoutExtension(fileName);
        protectedFileName = _protector.Protect(fileNameToProtect) + extension;
        senderOutputDirectory = Path.Combine(FileConfig.OutputDirectory, sender);
    }

    // Used for making new paths

}