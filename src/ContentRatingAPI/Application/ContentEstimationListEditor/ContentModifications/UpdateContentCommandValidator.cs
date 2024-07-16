using FluentValidation;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class UpdateContentCommandValidator : AbstractValidator<UpdateContentCommand>
    {
        public UpdateContentCommandValidator()
        {
            RuleFor(c => c.ContentType).IsInEnum();
            RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
            RuleFor(c => c.Path).NotEmpty();
            RuleFor(c => c.Id).NotEqual(Guid.Empty);
        }
    }
}
