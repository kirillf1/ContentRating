namespace ContentRating.Domain.AggregatesModel.ContentRatingAggregate
{
    public class ContentRatingService
    {
        private readonly IContentRatingRepository _contentRatingRepository;

        public ContentRatingService(IContentRatingRepository contentRatingRepository)
        {
            _contentRatingRepository = contentRatingRepository;
        }
        public async Task StartContentEstimation(IEnumerable<Guid> contentIds, Guid roomId, IEnumerable<RaterInvitation> invitations, Score minScore, Score maxScore)
        {
            var specification = new ContentRatingSpecification(new Score(0), new Score(10));
            foreach (var contentId in contentIds)
            {
                var contentRating = ContentRating.Create(contentId, roomId, specification);
                foreach (var invitation in invitations)
                {
                    contentRating.InviteRater(invitation);
                }
                _contentRatingRepository.Add(contentRating);
            }
            await _contentRatingRepository.UnitOfWork.SaveChangesAsync();
        }
        public async Task InviteRater(Guid roomId, RaterInvitation raterInvitation)
        {
            var ratings = await _contentRatingRepository.GetContentRatingsByRoom(roomId);
            foreach (var rating in ratings)
            {
                rating.InviteRater(raterInvitation);
                _contentRatingRepository.Update(rating);
            }
            await _contentRatingRepository.UnitOfWork.SaveChangesAsync();

        }
        public async Task CompleteContentEstimation(Guid roomId)
        {
            var ratings = await _contentRatingRepository.GetContentRatingsByRoom(roomId);
            foreach (var rating in ratings)
            {
                rating.CompleteEstimation();
                _contentRatingRepository.Update(rating);
            }
            await _contentRatingRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
