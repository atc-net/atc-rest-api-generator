using Demo.Api.Generated.Contracts.Users;
using FluentValidation;

namespace Demo.Domain.Validators.Users
{
    public class PostUserParametersValidator : AbstractValidator<PostUserParameters>
    {
        public PostUserParametersValidator()
        {
            RuleFor(x => x.Request.FirstName)
                .NotNull()
                .Length(2, 10)
                .Matches(@"^[A-Z]").WithMessage(x => $"{nameof(x.Request.FirstName)} has to start with an uppercase letter");

            RuleFor(x => x.Request.LastName)
                .NotNull()
                .Length(2, 30)
                .Matches(@"^[A-Z]").WithMessage(x => $"{nameof(x.Request.LastName)} has to start with an uppercase letter")
                .NotEqual(x => x.Request.FirstName);
        }
    }
}