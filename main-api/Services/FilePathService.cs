using System.Text;
using Microsoft.AspNetCore.DataProtection;

using api.Configuration;

namespace api.Services;

// Any work with file paths should be standardized through this service;
// The class should be used inside other public service classes and kept internal
internal class FilePathService {
    IDataProtector protector;

    internal FilePathService(IDataProtector protector) {
        this.protector = protector;
    }
    // Gets the protected file path inside the sender output directory
    internal void GetFilePath (
        string fileName, 
        string sender, 
        out string protectedFileName, 
        out string senderOutputDirectory) {

        string extension = Path.GetExtension(fileName).ToLowerInvariant();
        string fileNameToProtect = Path.GetFileNameWithoutExtension(fileName);
        protectedFileName = protector.Protect(fileNameToProtect) + extension;
        senderOutputDirectory = Path.Combine(FileConfig.OutputDirectory, sender);
    }

    // Used for making new paths
    internal void CreateFilePath (
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
}