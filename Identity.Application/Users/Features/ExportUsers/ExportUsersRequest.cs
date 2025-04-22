using FSH.Framework.Core.Paging;

namespace FSH.Framework.Core.Identity.Users.Features.ExportUsers;

public class ExportUsersRequest : BaseFilter
{
    public bool? IsActive { get; set; }
}
