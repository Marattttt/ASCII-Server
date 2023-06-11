using shared.DTOs;

namespace api.Services;

public interface IUsersManager {

    //Returns error message
    public Task<string> CreateUserAsync(FullUserInfoDTO dto);
    public Task<FullUserInfoDTO?> GetUserInfoAsync (int userId);
}