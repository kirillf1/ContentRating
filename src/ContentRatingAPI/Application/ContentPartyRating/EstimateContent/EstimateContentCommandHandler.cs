using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Exceptions;
using ContentRatingAPI.Application.ContentPartyRating.ContentRaterService;

namespace ContentRatingAPI.Application.ContentPartyRating.EstimateContent
{
    public class EstimateContentCommandHandler : IRequestHandler<EstimateContentCommand, Result>
    {
        private readonly IContentPartyRatingRepository contentRatingRepository;
        private readonly IContentRaterService contentRaterService;

        public EstimateContentCommandHandler(IContentPartyRatingRepository contentRatingRepository, IContentRaterService contentRaterService)
        {
            this.contentRatingRepository = contentRatingRepository;
            this.contentRaterService = contentRaterService;
        }
        public async Task<Result> Handle(EstimateContentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var contentRating = await contentRatingRepository.GetContentRating(request.ContentRatingId);
                if (contentRating is null)
                    return Result.NotFound();

                var raters = await contentRaterService.GetContentRatersForEstimation(contentRating.RoomId, request.EstimationInitiatorId, request.RaterForChangeScoreId);

                var estimation = new Estimation(raters.Initiator, raters.TargetRater, new Score(request.NewScore));
                contentRating.EstimateContent(estimation);
                contentRatingRepository.Update(contentRating);
                await contentRatingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (ForbiddenRatingOperationException ex)
            {
                return Result.Invalid(new ValidationError(ex.Message));
            }
        }
    }
}
