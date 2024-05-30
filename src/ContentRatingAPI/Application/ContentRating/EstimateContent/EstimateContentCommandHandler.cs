using ContentRating.Domain.AggregatesModel.ContentRatingAggregate;
using MediatR;

namespace ContentRatingAPI.Application.ContentRating.EstimateContent
{
    public class EstimateContentCommandHandler : IRequestHandler<EstimateContentCommand>
    {
        private readonly IContentRatingRepository contentRatingRepository;

        public EstimateContentCommandHandler(IContentRatingRepository contentRatingRepository)
        {
            this.contentRatingRepository = contentRatingRepository;
        }
        public async Task Handle(EstimateContentCommand request, CancellationToken cancellationToken)
        {
            var contentRating = await contentRatingRepository.GetContentRating(request.ContentId);

            var initiator = contentRating.Raters.Single(c=> c.Id == request.RaterInitiator);
            var rater = contentRating.Raters.Single(c => c.Id == request.TargetRater);
            var estimation = new Estimation(initiator, rater, new Score(request.NewScore));
            contentRating.EstimateContent(estimation);

            contentRatingRepository.Update(contentRating);
            await contentRatingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
