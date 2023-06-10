using shared.DTOs;

namespace api.Services;

public interface IUsers {

    //Returns error message
    public Task<string?> CreateUserAsync(FullUserInfoDTO dto);
    public Task<FullUserInfoDTO?> GetUserInfoAsync (int userId);
}