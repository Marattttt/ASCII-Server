using System.Diagnostics.CodeAnalysis;
using shared.DTOs;

namespace storage.Models;

public class User {
    public int UserId { get; set; }
    public string UserName { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
    public required ICollection<ImageData> Uploads { get; set; }
    public User() { }
    [SetsRequiredMembers]
    public User(FullUserInfoDTO dto) {
        UserId = dto.UserId;
        UserName = dto.UserName;
        Password = dto.Password;
        Uploads = new List<ImageData>();
    }
    public FullUserInfoDTO ToDto() {
        return new FullUserInfoDTO () {
            UserId = UserId,
            UserName = UserName,
            Password = Password
        };
    }
}