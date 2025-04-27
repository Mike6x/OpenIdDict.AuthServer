namespace Identity.Domain.Entities;

public class RefreshToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    public DateTime CreatedDate { get; set; }
    public string CreatedByIp { get; set; } = string.Empty;
    public DateTime? RevokedDate { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool IsActive => RevokedDate == null && !IsExpired;
}
