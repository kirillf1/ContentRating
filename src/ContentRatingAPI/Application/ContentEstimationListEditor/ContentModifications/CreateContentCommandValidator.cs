using FluentValidation;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class CreateContentCommandValidator : AbstractValidator<CreateContentCommand>
    {
        public CreateContentCommandValidator()
        {
            RuleFor(c=> c.ContentType).IsInEnum();
            RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
            RuleFor(c => c.Path).NotEmpty();
            RuleFor(c => c.ContentId).NotEqual(Guid.Empty);
        }
    }
}
