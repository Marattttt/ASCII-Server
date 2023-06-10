using System.Net;
using System.Text;
using System.Text.Json;

using shared.DTOs;
using shared.Config;

namespace api.Services;

public class StorageUsers : IUsers {
    public async Task<string?> CreateUserAsync(FullUserInfoDTO dto) {
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
                return responseMessage.Content.ToString();
            }
            return null;
        }
    }

    public async Task<FullUserInfoDTO?> GetUserInfoAsync (int userId) {
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
            return responseDto;
        }
    }
}