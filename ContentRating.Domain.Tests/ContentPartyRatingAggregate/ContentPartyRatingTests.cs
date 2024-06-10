using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Exceptions;
using Xunit;
using ContentRatingAggregateRoot = ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.ContentPartyRating;

namespace ContentRating.Domain.Tests.ContentPartyRatingAggregate
{
    public class ContentPartyRatingTests
    {
        [Fact]
        public void InviteRater_NewRater_RaterAdded()
        {
            var specification = CreateContentRatingSpecification();
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);

            var rater = new ContentRater(Guid.NewGuid(), RaterType.Admin);
            contentRating.AddNewRaterInContentEstimation(rater);


            Assert.Equal(contentRating.AverageContentScore, specification.MinScore);
            Assert.Equal(1, contentRating.RatersCount);
        }
        [Fact]
        public void RemoveRater_ExistingRater_RaterRemoved()
        {
            var specification = CreateContentRatingSpecification();
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);
            var rater = new ContentRater(Guid.NewGuid(), RaterType.Admin);
            contentRating.AddNewRaterInContentEstimation(rater);

            contentRating.RemoveRater(newRater);

            Assert.DoesNotContain(newRater, contentRating.Raters);
        }
        [Fact]
        public void InviteRater_ContentEstimated_ThrowForbiddenRatingOperationException()
        {
            var specification = CreateContentRatingSpecification();
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);

            contentRating.CompleteEstimation();
            var newInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Admin);

            Assert.Throws<ForbiddenRatingOperationException>(() => contentRating.InviteRater(newInvitation));
        }
        [Theory]
        [InlineData(0.3)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2.4)]
        [InlineData(10)]
        public void EstimateContent_ValidScore_ScoreChanged(double value)
        {
            var specification = CreateContentRatingSpecification(0, 10);
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);
            var expectedScore = new Score(value);

            var newInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Default);
            var newRater = contentRating.InviteRater(newInvitation);
            var newEstimation = new Estimation(newRater, newRater, expectedScore);
            contentRating.EstimateContent(newEstimation);

            var contentRatingChanged = contentRating.DomainEvents.OfType<ContentRatingChangedDomainEvent>().Single();
            Assert.Equal(contentRating.AverageContentScore, contentRatingChanged.AverageScore);
            Assert.Equal(contentRating.RoomId, contentRatingChanged.RoomId);
            Assert.Equal(expectedScore, newRater.CurrentScore);
        }
        [Theory]
        [InlineData(-1.4)]
        [InlineData(-2)]
        [InlineData(10.1)]
        public void EstimateContent_OutOfRangeScore_ThrowForbiddenRatingOperationException(double value)
        {
            var specification = CreateContentRatingSpecification(0, 10);
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);
            var expectedScore = new Score(value);

            var newInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Admin);
            var newRater = contentRating.InviteRater(newInvitation);
            var newEstimation = new Estimation(newRater, newRater, expectedScore);

            Assert.Throws<ForbiddenRatingOperationException>(() => contentRating.EstimateContent(newEstimation));
        }
        [Fact]
        public void EstimateContent_ForeignRatingWithoutPermission_ThrowForbiddenRatingOperationException()
        {
            var specification = CreateContentRatingSpecification();
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);
            var newScore = specification.MaxScore;

            var ownerInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Admin);
            var ownerRater = contentRating.InviteRater(ownerInvitation);
            var defaultRaterInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Default);
            var defaultRater = contentRating.InviteRater(defaultRaterInvitation);
            var estimation = new Estimation(defaultRater, ownerRater, newScore);

            Assert.Throws<ForbiddenRatingOperationException>(() => contentRating.EstimateContent(estimation));
        }
        [Fact]
        public void EstimateContent_OwnerChangeRatingDefaultUser_ContentRatingChanged()
        {
            var specification = CreateContentRatingSpecification();
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);
            var newScore = specification.MaxScore;

            var ownerInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Admin);
            var ownerRater = contentRating.InviteRater(ownerInvitation);
            var defaultRaterInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Default);
            var defaultRater = contentRating.InviteRater(defaultRaterInvitation);
            var estimation = new Estimation(ownerRater, defaultRater, newScore);
            contentRating.EstimateContent(estimation);

            var contentRatingChanged = contentRating.DomainEvents.OfType<ContentRatingChangedDomainEvent>().Single();
            Assert.Equal(contentRating.AverageContentScore, contentRatingChanged.AverageScore);
            Assert.Equal(contentRating.RoomId, contentRatingChanged.RoomId);
            Assert.Equal(defaultRater, contentRatingChanged.Rater);
            Assert.Equal(newScore, defaultRater.CurrentScore);
        }

        private static ContentRatingSpecification CreateContentRatingSpecification(double minScore = 0, double maxScore = 10)
        {
            return new ContentRatingSpecification(new Score(minScore), new Score(maxScore));
        }
    }
}
