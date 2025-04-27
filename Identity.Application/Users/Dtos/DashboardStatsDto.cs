namespace Identity.Application.Users.Dtos;
public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int OnlineUsers { get; set; }
    
    public int LockedUsers { get; set; }
    public int NewUsersToday { get; set; }
    public IEnumerable<RecentUserDto> RecentUsers { get; set; } = new List<RecentUserDto>();
}
