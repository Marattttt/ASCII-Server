using System.Net;
using System.Text;
using System.Text.Json;

using shared.DTOs;
using shared.Config;

namespace api.Services;

public class ApiUploadsManager : IUploadsManager {
    public Task<string?> UploadImage(FullUserInfoDTO user, ImageDataDTO image)
    {
        throw new NotImplementedException();
    }
}