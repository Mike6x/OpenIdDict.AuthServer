using FluentValidation;

namespace Identity.Application.Roles.Features.UpdatePermissions;
public class UpdatePermissionsValidator : AbstractValidator<UpdatePermissionsCommand>
{
    public UpdatePermissionsValidator()
    {
        RuleFor(r => r.RoleId)
            .NotEmpty();
        RuleFor(r => r.Permissions)
            .NotNull();
    }
}
