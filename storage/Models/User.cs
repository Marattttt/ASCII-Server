namespace storage.Models;

public class User {
    public int UserId { get; set; }
    public string UserName { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
    public required ICollection<ImageData> Uploads { get; set; }
}