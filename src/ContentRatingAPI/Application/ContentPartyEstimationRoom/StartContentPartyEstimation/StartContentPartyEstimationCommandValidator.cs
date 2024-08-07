using FluentValidation;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation
{
    public class StartContentPartyEstimationCommandValidator : AbstractValidator<StartContentPartyEstimationCommand>
    {
        public StartContentPartyEstimationCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().Length(3, 50);
            RuleFor(c => c.ContentListId).NotEqual(Guid.Empty);
            RuleFor(c => c.CreatorName).NotEmpty();
            RuleFor(c=> c.RoomId).NotEqual(Guid.Empty);
        }
    }
}
