using FluentValidation;

namespace ContentRatingAPI.Application.Identity.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty();
            RuleFor(x => x.AccessToken).NotEmpty();
        }
    }
}
