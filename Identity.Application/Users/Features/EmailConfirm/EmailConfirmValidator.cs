using FluentValidation;

namespace Identity.Application.Users.Features.EmailConfirm;

public class EmailConfirmValidator : AbstractValidator<EmailConfirmCommand>
{
    public EmailConfirmValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Tenant).NotEmpty();
    }
}
