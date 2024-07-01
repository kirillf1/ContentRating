using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.Shared.RoomStates;
using Xunit;

namespace ContentRating.Domain.Tests.ContentPartyEstimationRoomTest
{
    public class ContentPartyEstimationRoomTests
    {
        [Fact]
        public void InviteRater_ByAdmin_AddedUserInvitedEvent()
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin, "testName");
            var contentPartyEstimationRoom = ContentPartyEstimationRoom.Create(Guid.NewGuid(), creator, []);

            var newUser = new Rater(Guid.NewGuid(), RoleType.Default, "new_rater");
            contentPartyEstimationRoom.InviteRater(newUser, creator.Id);

            var raterInvitedEvent = contentPartyEstimationRoom.DomainEvents.OfType<RaterInvitedDomainEvent>().Single();
            Assert.Equal(newUser, raterInvitedEvent.Rater);
            Assert.Equal(contentPartyEstimationRoom.Id, raterInvitedEvent.RoomId);
        }

        [Theory]
        [InlineData(RoleType.Default)]
        [InlineData(RoleType.Mock)]
        public void InviteUser_ForbiddenInviteRole_ThrowForbiddenRoomOperationException(RoleType forbiddenInviteRole)
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin, "new_rater");
            var inviteInitiator = new Rater(Guid.NewGuid(), forbiddenInviteRole, "new_rater");
            var room = ContentPartyEstimationRoom.Create(Guid.NewGuid(), creator, [], otherInvitedRaters: [inviteInitiator]);

            var newUser = new Rater(Guid.NewGuid(), RoleType.Default, "new_rater");

            Assert.Throws<ForbiddenRoomOperationException>(() => room.InviteRater(newUser, inviteInitiator.Id));

        }
        [Fact]
        public void KickUser_ByAdmin_AddUserKickedEvent()
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin, "new_rater");
            var raterForKick = new Rater(Guid.NewGuid(), RoleType.Default, "new_rater");
            var room = ContentPartyEstimationRoom.Create(Guid.NewGuid(), creator, [], otherInvitedRaters: [raterForKick]);

            room.KickRater(raterForKick.Id, creator.Id);

            var userKickedEvent = room.DomainEvents.OfType<RaterKickedDomainEvent>().Single();
            Assert.Equal(raterForKick, userKickedEvent.KickedRater);
            Assert.Equal(room.Id, userKickedEvent.RoomId);
        }
        [Theory]
        [InlineData(RoleType.Default)]
        [InlineData(RoleType.Mock)]
        public void KickUser_ForbiddenKickRole_ThrowForbiddenRoomOperationException(RoleType forbiddenInviteRole)
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin, "new_rater");
            var kickInitiator = new Rater(Guid.NewGuid(), forbiddenInviteRole, "new_rater");
            var kickTarget = new Rater(Guid.NewGuid(), RoleType.Default, "new_rater");
            var room = ContentPartyEstimationRoom.Create(Guid.NewGuid(), creator, [], otherInvitedRaters: [kickInitiator, kickTarget]);

            Assert.Throws<ForbiddenRoomOperationException>(() => room.KickRater(kickTarget.Id, kickInitiator.Id));
        }

        [Fact]
        public void KickUser_ContentEstimated_ThrowInvalidRoomStageOperationException()
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin, "test");
            var userForKick = new Rater(Guid.NewGuid(), RoleType.Default, "test");
            var room = ContentPartyEstimationRoom.Create(Guid.NewGuid(), creator, [], otherInvitedRaters: [userForKick]);

            room.CompleteContentEstimation(creator);

            Assert.Throws<InvalidRoomStageOperationException>(() => room.KickRater(userForKick.Id, creator.Id));
        }
     
    }
}
