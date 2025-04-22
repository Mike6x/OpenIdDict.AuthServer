namespace Identity.Application.Claims.Features.Update;

public class AssignClaimsCommand
{
    public string Owner { get; set; } = string.Empty;
    public List<ClaimViewModel> Claims { get; set; } = [];
}