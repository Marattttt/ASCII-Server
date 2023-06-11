using System.Net;
using System.Text;
using System.Text.Json;

using shared.DTOs;
using shared.Config;
using shared.DataChecking;

namespace api.Services;

public class StorageUsersManager : IUsersManager {
    private List<FullUserInfoDTO> trackedUsers;
    
    public StorageUsersManager() {
        trackedUsers = new List<FullUserInfoDTO>();
    }
    public async Task<string> CreateUserAsync(FullUserInfoDTO dto) {
        string errorMessage = UserDataChecker.CheckFullUserDto(dto);
        if (errorMessage != String.Empty) {
            throw new ArgumentException (errorMessage);
        }
        
        using (HttpClient client = new HttpClient()) {
            StringContent content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage responseMessage = await client.PostAsync(
                CommunicationUrls.StorageUrl + "user/new",
                content
            );
            if (responseMessage.StatusCode != HttpStatusCode.NoContent) {
                return responseMessage.Content.ToString() ?? "Unknown creation error";
            }
            if (!trackedUsers.Any(usr => usr == dto))
                trackedUsers.Add(dto);
            return String.Empty;
        }
    }

    public async Task<FullUserInfoDTO?> GetUserInfoAsync (int userId) {
        FullUserInfoDTO? tracked = trackedUsers.FirstOrDefault(
            dto => dto?.UserId == userId, null);
        if (tracked is not null) {
            return tracked;
        }
        using (HttpClient client = new HttpClient()) {
            HttpResponseMessage responseMessage = await client.GetAsync(
                CommunicationUrls.StorageUrl + $"user/{userId}"
            );
            if (responseMessage.StatusCode != HttpStatusCode.NoContent) {
                return null;
            }
            FullUserInfoDTO? responseDto = JsonSerializer.Deserialize<FullUserInfoDTO>(
                await responseMessage.Content.ReadAsStreamAsync());
            if (responseDto is null) {
                return null;
            }
            trackedUsers.Add(responseDto);
            return responseDto;
        }
    }
}