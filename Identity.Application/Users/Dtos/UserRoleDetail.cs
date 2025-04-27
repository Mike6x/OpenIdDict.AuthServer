namespace Identity.Application.Users.Dtos;

//UserRoleViewModel
public class UserRoleDetail
{
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
}
