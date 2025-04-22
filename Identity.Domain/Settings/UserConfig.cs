namespace Identity.Domain.Models;

public class UserConfig
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    public string Role { get; set; } = string.Empty;
}
