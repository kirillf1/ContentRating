using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.ContentPartyRating.ContentRaterService;

namespace ContentRatingAPI.Application.ContentPartyRating.EstimateContent
{
    public class EstimateContentCommandHandler : IRequestHandler<EstimateContentCommand>
    {
        private readonly IContentPartyRatingRepository contentRatingRepository;
        private readonly IContentRaterService contentRaterService;

        public EstimateContentCommandHandler(IContentPartyRatingRepository contentRatingRepository, IContentRaterService contentRaterService)
        {
            this.contentRatingRepository = contentRatingRepository;
            this.contentRaterService = contentRaterService;
        }
        public async Task Handle(EstimateContentCommand request, CancellationToken cancellationToken)
        {
            var contentRating = await contentRatingRepository.GetContentRating(request.ContentRatingId);
            var raters = await contentRaterService.GetContentRatersForEstimation(contentRating.RoomId, request.EstimationInitiatorId, request.RaterForChangeScoreId);

            var estimation = new Estimation(raters.Initiator, raters.TargetRater, new Score(request.NewScore));
            contentRating.EstimateContent(estimation);
             
            contentRatingRepository.Update(contentRating);
            await contentRatingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
