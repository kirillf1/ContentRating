// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class ContentPartyRatingService
    {
        private readonly IContentPartyRatingRepository _contentRatingRepository;

        public ContentPartyRatingService(IContentPartyRatingRepository contentRatingRepository)
        {
            _contentRatingRepository = contentRatingRepository;
        }

        public async Task StartContentEstimation(
            IEnumerable<Guid> contentIds,
            Guid roomId,
            IEnumerable<ContentRater> raters,
            Score minScore,
            Score maxScore
        )
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

        public async Task RemoveRaterScoreInContentRatingList(Guid roomId, ContentRater rater)
        {
            var ratings = await _contentRatingRepository.GetContentRatingsByRoom(roomId);
            foreach (var rating in ratings)
            {
                rating.RemoveRaterFromContentEstimation(rater);
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

        public async Task ChangeRatingSpecification(Guid roomId, Score newMinScore, Score newMaxScore)
        {
            var ratings = await _contentRatingRepository.GetContentRatingsByRoom(roomId);
            foreach (var rating in ratings)
            {
                rating.ChangeEstimationSpecification(new ContentRatingSpecification(newMinScore, newMaxScore));
                _contentRatingRepository.Update(rating);
            }
            await _contentRatingRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
