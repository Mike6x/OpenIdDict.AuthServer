using System.Collections.ObjectModel;

namespace Shared.Authorization;

public static class AppPermissions
{
    private static readonly AppPermission[] AllPermissions =
    [
        //tenants
        // new("View Tenants", AppActions.View, AppResources.Tenants, IsRoot: true),
        // new("Search Tenants", AppActions.Search, AppResources.Tenants, IsRoot: true),
        // new("Create Tenants", AppActions.Create, AppResources.Tenants, IsRoot: true),
        // new("Update Tenants", AppActions.Update, AppResources.Tenants, IsRoot: true),
        // new("Delete Tenants", AppActions.Delete, AppResources.Tenants, IsRoot: true),
        // new("Export Tenants", AppActions.Export, AppResources.Tenants, IsRoot: true),
        // new("Import Tenants", AppActions.Import, AppResources.Tenants, IsRoot: true),
        //
        // new("Upgrade Tenant Subscription", AppActions.UpgradeSubscription, AppResources.Tenants, IsRoot: true),

        //identity
        new("View Users", AppActions.View, AppResources.Users),
        new("Search Users", AppActions.Search, AppResources.Users),
        new("Create Users", AppActions.Create, AppResources.Users),
        new("Update Users", AppActions.Update, AppResources.Users),
        new("Delete Users", AppActions.Delete, AppResources.Users),
        new("Export Users", AppActions.Export, AppResources.Users),
        new("Import Users", AppActions.Import, AppResources.Users),

        new("View UserRoles", AppActions.View, AppResources.UserRoles),
        new("Update UserRoles", AppActions.Update, AppResources.UserRoles),

        new("View Roles", AppActions.View, AppResources.Roles),
        new("Search Roles", AppActions.Search, AppResources.Roles),
        new("Create Roles", AppActions.Create, AppResources.Roles),
        new("Update Roles", AppActions.Update, AppResources.Roles),
        new("Delete Roles", AppActions.Delete, AppResources.Roles),
        new("Export Roles", AppActions.Export, AppResources.Roles),
        new("Import Roles", AppActions.Import, AppResources.Roles),

        new("View RoleClaims", AppActions.View, AppResources.RoleClaims),
        new("Update RoleClaims", AppActions.Update, AppResources.RoleClaims)
    ];

    public static IReadOnlyList<AppPermission> All { get; } = new ReadOnlyCollection<AppPermission>(AllPermissions);
    public static IReadOnlyList<AppPermission> Root { get; } = new ReadOnlyCollection<AppPermission>(AllPermissions.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<AppPermission> Admin { get; } = new ReadOnlyCollection<AppPermission>(AllPermissions.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<AppPermission> Basic { get; } = new ReadOnlyCollection<AppPermission>(AllPermissions.Where(p => p.IsBasic).ToArray());
}


public record AppPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);

    private static string NameFor(string action, string resource)
    {
        return $"Permissions.{resource}.{action}";
    }
}
