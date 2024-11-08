// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.Shared;
using Moq;
using Xunit;
using ContentRatingAggregateRoot = ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.ContentPartyRating;

namespace ContentRating.Domain.Tests.ContentPartyRatingAggregateTest
{
    public class ContentPartyRatingServiceTests
    {
        [Fact]
        public async Task StartEstimation_ValidContentWithRaters_ContentRatingSavedWithMinScore()
        {
            var repository = new Mock<IContentPartyRatingRepository>();
            var maxScore = new Score(5);
            var minScore = new Score(0);
            var expectedAverageScore = minScore;
            var unitOfwork = Mock.Of<IUnitOfWork>();
            repository.Setup(c => c.UnitOfWork).Returns(unitOfwork);
            var newRaters = new List<ContentRater>()
            {
                new(Guid.NewGuid(), RaterType.Admin),
                new(Guid.NewGuid(), RaterType.Mock),
                new(Guid.NewGuid(), RaterType.Default),
            };
            var contentIds = new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
            };
            var expectedRoomId = Guid.NewGuid();

            var contentRatingService = new ContentPartyRatingService(
                repository.Object
            );
            await contentRatingService.StartContentEstimation(
                contentIds,
                expectedRoomId,
                newRaters,
                minScore,
                maxScore
            );

            repository.Verify(
                c =>
                    c.Add(
                        It.Is<ContentRatingAggregateRoot>(c =>
                            contentIds.Contains(c.ContentId)
                            && c.RaterScores.Count == newRaters.Count
                            && c.AverageContentScore.Equals(
                                expectedAverageScore
                            )
                        )
                    ),
                Times.Exactly(contentIds.Count)
            );
            Mock.Get(unitOfwork)
                .Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CompleteContentEstimation_SameRoomId_ContentStateIsEstimated()
        {
            var repository = new Mock<IContentPartyRatingRepository>();
            var contentRatingList = new List<ContentRatingAggregateRoot>();
            var unitOfwork = Mock.Of<IUnitOfWork>();
            var contentRatings = CreateContentRatingsWithSameRaters(4).ToList();
            repository.Setup(c => c.UnitOfWork).Returns(unitOfwork);
            repository
                .Setup(c => c.GetContentRatingsByRoom(It.IsAny<Guid>()))
                .Returns(Task.FromResult(contentRatings.AsEnumerable()));
            var roomId = Guid.NewGuid();

            var contentRatingService = new ContentPartyRatingService(
                repository.Object
            );
            await contentRatingService.CompleteContentEstimation(roomId);

            repository.Verify(
                c => c.GetContentRatingsByRoom(roomId),
                Times.Once
            );
            repository.Verify(
                c =>
                    c.Update(
                        It.Is<ContentRatingAggregateRoot>(c =>
                            c.IsContentEstimated == true
                        )
                    ),
                Times.Exactly(contentRatings.Count)
            );
            Mock.Get(unitOfwork)
                .Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        private IEnumerable<ContentRatingAggregateRoot> CreateContentRatingsWithSameRaters(
            int contentRatingCount
        )
        {
            var raters = new List<ContentRater>()
            {
                new(Guid.NewGuid(), RaterType.Admin),
                new(Guid.NewGuid(), RaterType.Mock),
                new(Guid.NewGuid(), RaterType.Default),
            };
            var roomId = Guid.NewGuid();
            var specification = new ContentRatingSpecification(
                new Score(0),
                new Score(5)
            );
            for (int i = 0; i < contentRatingCount; i++)
            {
                var contentRating = ContentRatingAggregateRoot.Create(
                    Guid.NewGuid(),
                    roomId,
                    specification
                );
                foreach (var invite in raters)
                {
                    contentRating.AddNewRaterInContentEstimation(invite);
                }
                yield return contentRating;
            }
        }
    }
}
