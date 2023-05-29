using storage.Data;
using storage.Models;

namespace storage.Services;

public class ImagesService {
    private UserDbContext _userContext;
    public ImagesService(UserDbContext userDbContext) {
        _userContext = userDbContext;
    }

    public async Task<User?> GetUser (int userId) {
        User? user = await _userContext.Users.FindAsync(userId);
        return user;
    }
}