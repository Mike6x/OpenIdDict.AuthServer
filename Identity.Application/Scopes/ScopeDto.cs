using System.ComponentModel.DataAnnotations;

namespace Identity.Application.Scopes;

public class ScopeDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> Resources { get; set; } = [];
}