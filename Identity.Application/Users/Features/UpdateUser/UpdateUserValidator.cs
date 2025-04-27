using FluentValidation;

namespace Identity.Application.Users.Features.UpdateUser
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator(IUserService userService)
        {
            RuleFor(p => p.Id)
                .NotEmpty();

            RuleFor(p => p.FirstName)
                .NotEmpty()
                .MaximumLength(75);

            RuleFor(p => p.LastName)
                .NotEmpty()
                .MaximumLength(75);

            RuleFor(p => p.Email)
                .NotEmpty()
                .EmailAddress()
                    .WithMessage("Invalid Email Address.")
              //  .MustAsync(async (user, email, _) => !await userService.ExistsWithEmailAsync(email, user.Id))
                    .WithMessage((_, email) => $"Email {email} is already registered.");

            // RuleFor(u => u.PhoneNumber).Cascade(CascadeMode.Stop)
            //     .MustAsync(async (user, phone, _) => !await userService.ExistsWithPhoneNumberAsync(phone!, user.Id))
            //         .WithMessage((_, phone) => $"Phone number {phone} is already registered.")
            //         .Unless(u => string.IsNullOrWhiteSpace(u.PhoneNumber))
        }
    }
}