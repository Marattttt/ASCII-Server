using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using shared.DTOs;
using shared.Config;
using shared.DataChecking;

namespace api.Services;

public class StorageUsersManager : IUsersManager {
    private List<FullUserInfoDTO> _trackedUsers;
    
    public StorageUsersManager() {
        _trackedUsers = new List<FullUserInfoDTO>();
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
                CommunicationUrls.StorageUrl + "users/new",
                content
            );
            if (responseMessage.StatusCode != HttpStatusCode.NoContent) {
                return responseMessage.Content.ToString() ?? "Unknown creation error";
            }
            if (!_trackedUsers.Any(usr => usr == dto))
                _trackedUsers.Add(dto);
            return String.Empty;
        }
    }

    public async Task<FullUserInfoDTO?> GetUserInfoAsync (int userId) {
        FullUserInfoDTO? tracked = _trackedUsers.FirstOrDefault(
            dto => dto?.UserId == userId, null);
        if (tracked is not null) {
            return tracked;
        }
        using (HttpClient client = new HttpClient()) {
            HttpResponseMessage responseMessage = await client.GetAsync(
                CommunicationUrls.StorageUrl + $"user/{userId}"
            );
            if (!responseMessage.IsSuccessStatusCode) {
                return null;
            }
            Console.WriteLine(await responseMessage.Content.ReadAsStringAsync());
            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            };

            FullUserInfoDTO? responseDto = JsonSerializer.Deserialize<FullUserInfoDTO>(
                await responseMessage.Content.ReadAsStringAsync(),
                options);
            if (responseDto is null) {
                return null;
            }
            _trackedUsers.Add(responseDto);
            return responseDto;
        }
    }

    public async Task DeleteUserAsync (int userId) {
        using (HttpClient client = new HttpClient()) {
            HttpResponseMessage responseMessage = await client.DeleteAsync(
                CommunicationUrls.StorageUrl + $"user/{userId}"
            );
            _trackedUsers.RemoveAll(u => u.UserId == userId);
            return;
        }
    }
}