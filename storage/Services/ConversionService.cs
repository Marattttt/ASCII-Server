using System.Threading.Tasks;
using System.Net.Mime;

using shared.DTOs;
using storage.Models;

namespace storage.Services;

public static class ConversionService {
    public static ImageDataDTO ImageDataToImageDataDto(ImageData data) {
        return new ImageDataDTO() {
            UserId = data.UserId,
            FileName = data.FileName,
            FileType = data.FileType,
            ASCII = data.ASCII,
            Content = data.Content
        };
    }
    public static ImageData? ImageDtoToImageData(ImageDataDTO dto) {
        if (dto.Content is null) {
            return null;
        }

        ImageData data;
        try {
            data = new ImageData() {
                FileName = dto.FileName.Trim().ToLowerInvariant(),
                FileType = dto.FileType.Trim().ToLowerInvariant(),
                Content = dto.Content,
                ASCII = dto.ASCII,
                UserId = dto.UserId,
                Owner = new User(){ Uploads = new List<ImageData>() }
            };
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            return null;
        }
        return data;
    }

    public static User? FullUserInfoDtoToUser(FullUserInfoDTO dto) {
        return new User() {
            UserId = dto.UserId,
            UserName = dto.UserName,
            Password = dto.Password,
            Uploads = new List<ImageData>()
        };
    }
}