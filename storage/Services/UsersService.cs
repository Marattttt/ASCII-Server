using FileTypeChecker;
using FileTypeChecker.Abstracts;

using shared.DTOs;
using storage.Data;
using storage.Models;
using Microsoft.EntityFrameworkCore;

namespace storage.Services;

public class UsersService {
    private UserDbContext _userContext;
    public UsersService(UserDbContext userDbContext) {
        _userContext = userDbContext;
    }

    public async Task<User?> GetUserAsync (int userId, bool loadUploads = false) {
        User? user;
        if (loadUploads) {
            user = await _userContext.Users
                .Include(u => u.Uploads)
                .FirstAsync(u => u.UserId == userId);
        } else {
            user = await _userContext.Users.FindAsync(userId);
        }
        return user;
    }
    public async Task<User?> CreateUserAsync (FullUserInfoDTO dto) {
        User? existingUser = await GetUserAsync(dto.UserId);
        if (existingUser is not null) {
            throw new ArgumentException("User already exists");
        }
        User? newUser = new User(dto);
        
        _userContext.Add(newUser);
        await _userContext.SaveChangesAsync();
        return newUser;
    }

    public async Task<UsersServiceResult> SaveImageDataAsync (User user, ImageData img) {
        
        using (var memStream = new MemoryStream(img.Content)) {
            bool isRecognizableType = FileTypeValidator.IsImage(memStream);
            if (!isRecognizableType) {
                return UsersServiceResult.FileTypeNotAllowed;
            }
            IFileType fileType = FileTypeValidator.GetFileType(memStream);
            img.FileType = fileType.Extension;
        }
        if (user is null) {
            // return UsersServiceResult.UserNotFound;
            user = new User(){ Uploads = new List<ImageData>() };
            _userContext.Users.Add(user);
        }

        if (user.Uploads is null) {
            user.Uploads = new List<ImageData>();
        }

        if (user.Uploads.Any(i => i.ImageId == img.ImageId)){
            ImageData existingImage = user.Uploads.First(i => i.ImageId == img.ImageId);
            existingImage.CopyData(img);
        } else {
            user.Uploads.Add(img);
        }
        
        await _userContext.SaveChangesAsync();
        return UsersServiceResult.Success;
    }
    public (UsersServiceResult result, ImageData? imageData) GetImageData (
        User user, string fileName) {

        UsersServiceResult result = UsersServiceResult.Success;

        List<ImageData> imageData = _userContext.Entry(user).Collection(u => u.Uploads)
                                .Query()
                                .Where(img => img.FileName == fileName)
                                .ToList();
        if (imageData.Count() == 0) {
            result = UsersServiceResult.ImageNotFound;
        }

        if (result != UsersServiceResult.Success) {
            return (result, null);
        }
        return (result, imageData[0]);
    }

    public async Task DeleteImage (User user, string fileName) {
        ImageData? imageData = user.Uploads.FirstOrDefault(
            i => i?.FileName == fileName, null);
        if (imageData is null) {
            return;
        }
        _userContext.Remove(imageData);
        await _userContext.SaveChangesAsync();
    }

    public async Task DeleteUser (User user) {
        _userContext.Users.Remove(user);
        await _userContext.SaveChangesAsync();
    }
}