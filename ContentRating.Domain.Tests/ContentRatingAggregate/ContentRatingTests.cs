using ContentRating.Domain.AggregatesModel.ContentRatingAggregate;
using ContentRating.Domain.AggregatesModel.ContentRatingAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentRatingAggregate.Exceptions;
using Xunit;
using ContentRatingAggregateRoot = ContentRating.Domain.AggregatesModel.ContentRatingAggregate.ContentRating;

namespace ContentRating.Domain.Tests.ContentRatingAggregate
{
    public class ContentRatingTests
    {
        [Fact]
        public void InviteRater_NewRater_RaterAdded()
        {
            var specification = CreateContentRatingSpecification();
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);

            var newInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Owner);
            var newRater = contentRating.InviteRater(newInvitation);

            Assert.Contains(newRater, contentRating.Raters);
            Assert.Equal(newRater.CurrentScore, specification.MinScore);
            Assert.Equal(newInvitation.Id, newRater.Id);
        }
        [Fact]
        public void RemoveRater_ExistingRater_RaterRemoved()
        {
            var specification = CreateContentRatingSpecification();
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);
            var newInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Owner);
            var newRater = contentRating.InviteRater(newInvitation);

            contentRating.RemoveRater(newRater);

            Assert.DoesNotContain(newRater, contentRating.Raters);
        }
        [Fact]
        public void InviteRater_ContentEstimated_ThrowForbiddenRatingOperationException()
        {
            var specification = CreateContentRatingSpecification();
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);

            contentRating.CompleteEstimation();
            var newInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Owner);

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

            var newInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Owner);
            var newRater = contentRating.InviteRater(newInvitation);
            var newEstimation = new Estimation(newRater, newRater, expectedScore);
            
            Assert.Throws<ForbiddenRatingOperationException> (()=> contentRating.EstimateContent(newEstimation));
        }
        [Fact]
        public void EstimateContent_ForeignRatingWithoutPermission_ThrowForbiddenRatingOperationException()
        {
            var specification = CreateContentRatingSpecification();
            var contentRating = ContentRatingAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), specification);
            var newScore = specification.MaxScore;
            
            var ownerInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Owner);
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

            var ownerInvitation = new RaterInvitation(Guid.NewGuid(), RaterType.Owner);
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
