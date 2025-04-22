namespace Identity.Domain.Models;

public class RefreshToken
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    public DateTime CreatedDate { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? RevokedDate { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool IsActive => RevokedDate == null && !IsExpired;
}
