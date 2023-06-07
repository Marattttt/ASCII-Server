using FileTypeChecker;
using FileTypeChecker.Abstracts;

using shared.DTOs;
using storage.Data;
using storage.Models;

namespace storage.Services;

public class UsersService {
    private UserDbContext _userContext;
    public UsersService(UserDbContext userDbContext) {
        _userContext = userDbContext;
    }

    public async Task<User?> GetUserAsync (int userId) {
        User? user;
        user = await _userContext.Users.FindAsync(userId);
        return user;
    }

    public async Task<User?> CreateUserAsync (FullUserInfoDTO dto) {
        User? existingUser = await GetUserAsync(dto.UserId);
        if (existingUser is not null) {
            throw new ArgumentException("User already exists");
        }
        User? newUser = ConversionService.FullUserInfoDtoToUser(dto);
        if (newUser is null) {
            return null;
        }
        _userContext.Add(newUser);
        await _userContext.SaveChangesAsync();
        return newUser;
    }

    public async Task<UsersServiceResult> SaveImageDataAsync (User user, ImageData img) {
        img.FileName.Trim().ToLowerInvariant();

        using (var memStream = new MemoryStream(img.Content)) {
            bool isRecognizableType = FileTypeValidator.IsImage(memStream);
            if (!isRecognizableType) {
                return UsersServiceResult.FileTypeNotAllowed;
            }
            IFileType fileType = FileTypeValidator.GetFileType(memStream);
            Console.WriteLine(fileType.ToString());
            Console.WriteLine(fileType.Name);
            Console.WriteLine(fileType.Extension);
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
    public (UsersServiceResult result, ImageData? imageData) GetImageData (User user, string fileName) {
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
}