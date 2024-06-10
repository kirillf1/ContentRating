﻿namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class ContentPartyRatingService
    {
        private readonly IContentPartyRatingRepository _contentRatingRepository;

        public ContentPartyRatingService(IContentPartyRatingRepository contentRatingRepository)
        {
            _contentRatingRepository = contentRatingRepository;
        }
        public async Task StartContentEstimation(IEnumerable<Guid> contentIds, Guid roomId, IEnumerable<ContentRater> raters, Score minScore, Score maxScore)
        {
            var specification = new ContentRatingSpecification(minScore, maxScore);
            foreach (var contentId in contentIds)
            {
                var contentRating = ContentPartyRating.Create(contentId, roomId, specification);
                foreach (var rater in raters)
                {
                    contentRating.AddNewRaterInContentEstimation(rater);
                }
                _contentRatingRepository.Add(contentRating);
            }
            await _contentRatingRepository.UnitOfWork.SaveChangesAsync();
        }
        public async Task AddNewRaterScoreInContentRatingList(Guid roomId, ContentRater newRater)
        {
            var ratings = await _contentRatingRepository.GetContentRatingsByRoom(roomId);
            foreach (var rating in ratings)
            {
                rating.AddNewRaterInContentEstimation(newRater);
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
