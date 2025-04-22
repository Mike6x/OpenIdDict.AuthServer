namespace IdentityServer.Core.DTOs;
public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public IEnumerable<RecentUserDto> RecentUsers { get; set; } = new List<RecentUserDto>();
}
