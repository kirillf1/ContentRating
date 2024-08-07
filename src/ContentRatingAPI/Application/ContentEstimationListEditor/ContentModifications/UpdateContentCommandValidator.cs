using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using FluentValidation;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class UpdateContentCommandValidator : AbstractValidator<UpdateContentCommand>
    {
        public UpdateContentCommandValidator()
        {
            RuleFor(c => c.ContentType).IsInEnum();
            RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
            RuleFor(c => c.Url).Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.Url));
            RuleFor(c => c.Id).NotEqual(Guid.Empty);
        }
    }
}
