using FluentValidation;

namespace Identity.Application.Users.Features.CreateUser;


public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator(IUserService userService)
    {

        RuleFor(u => u.FirstName)
            .NotEmpty()
            .MaximumLength(75);

        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid Email Address.")
            // .MustAsync(async (user, email, _) => !await userService.ExistsWithEmailAsync(email))
            .WithMessage((_, email) => $"Email {email} is already registered.");

        RuleFor(u => u.PhoneNumber).Cascade(CascadeMode.Stop)
            .NotEmpty()
            // .MustAsync(async (user, phone, _) => !await userService.ExistsWithPhoneNumberAsync(phone!))
            .WithMessage((_, phone) => $"Phone number {phone} is already registered.");
        //.Unless(u => string.IsNullOrWhiteSpace(u.PhoneNumber))
    }
}
