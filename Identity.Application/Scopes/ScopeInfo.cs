namespace Identity.Application.Scopes.Features;

public class ScopeInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Properties { get; set; }
    public List<string> Resources { get; set; } = [];
}