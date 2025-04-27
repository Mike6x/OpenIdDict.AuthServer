using Framework.Core.Paging;

namespace Identity.Application.Users.Features.ExportUsers;

public class ExportUsersRequest : BaseFilter
{
    public bool? IsActive { get; set; }
}
