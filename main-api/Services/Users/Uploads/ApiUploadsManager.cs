using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;

using shared.DTOs;
using shared.Config;

namespace api.Services;

public class ApiUploadsManager : IUploadsManager {
    public async Task<string?> UploadImageAsync(ImageDataDTO dto) {
        using (HttpClient client = new HttpClient()) {
            HttpContent content = new StringContent(
                JsonSerializer.Serialize(dto).ToString(),
                Encoding.UTF8,
                "application/json"
            );
            HttpResponseMessage responseMessage = await client.PostAsync(
                CommunicationUrls.StorageUrl + "user/images/new",
                content
            );

            if (responseMessage.StatusCode == HttpStatusCode.NoContent) {
                return null;
            }

            return await responseMessage.Content.ReadAsStringAsync();
        }
    }
}