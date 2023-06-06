using FileTypeChecker;
using FileTypeChecker.Abstracts;

using shared.DTOs;
using storage.Data;
using storage.Models;

namespace storage.Services;

public class ImagesService {
    private UserDbContext _userContext;
    public ImagesService(UserDbContext userDbContext) {
        _userContext = userDbContext;
    }

    public async Task<User?> GetUserAsync (int userId) {
        User? user;
        user = await _userContext.Users.FindAsync(userId);
        return user;
    }

    public async Task<ImagesServiceResult> SaveImageDataAsync (User user, ImageData img) {
        img.FileName.Trim().ToLowerInvariant();

        using (var memStream = new MemoryStream(img.Content)) {
            bool isRecognizableType = FileTypeValidator.IsImage(memStream);
            if (!isRecognizableType) {
                return ImagesServiceResult.FileTypeNotAllowed;
            }
            IFileType fileType = FileTypeValidator.GetFileType(memStream);
            Console.WriteLine(fileType.ToString());
            Console.WriteLine(fileType.Name);
            Console.WriteLine(fileType.Extension);
        }
        if (user is null) {
            // return ImagesServiceResult.UserNotFound;
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
        return ImagesServiceResult.Success;
    }
    public (ImagesServiceResult result, ImageData? imageData) GetImageData (User user, string fileName) {
        ImagesServiceResult result = ImagesServiceResult.Success;

        List<ImageData> imageData = _userContext.Entry(user).Collection(u => u.Uploads)
                                .Query()
                                .Where(img => img.FileName == fileName)
                                .ToList();

        if (imageData.Count() == 0) {
            result = ImagesServiceResult.ImageNotFound;
        }

        if (result != ImagesServiceResult.Success) {
            return (result, null);
        }


        return (result, imageData[0]);
    }
}