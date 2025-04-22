using FluentValidation;

namespace Identity.Application.Claims.Features.Update;

public class UpdateClaimsValidator : AbstractValidator<AssignClaimsCommand>
{
    public UpdateClaimsValidator()
    {
        RuleFor(r => r.Owner)
            .NotEmpty();
        RuleFor(r => r.Claims.Count > 0)
            .NotNull();
    }
}