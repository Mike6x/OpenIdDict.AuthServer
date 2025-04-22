using System.Collections.ObjectModel;

namespace Shared.Authorization;

public class AppRoles
{
    public const string Admin = nameof(Admin);
    public const string Manager = nameof(Manager);
    public const string Basic = nameof(Basic);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>([
        Admin,
        Manager,
        Basic
    ]);

    public static bool IsDefault(string roleName)
    {
        return DefaultRoles.Any(r => r == roleName);
    }
}