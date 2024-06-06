using ContentRating.Domain.AggregatesModel.ContentPartyRatingRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingRoomAggregate.Events;
using ContentRating.Domain.Shared.RoomStates;
using Xunit;

namespace ContentRating.Domain.Tests.RoomAccessControlAggregate
{
    public class RoomAccessControlTests
    {
        [Fact]
        public void InviteUser_ByAdmin_AddedUserInvitedEvent()
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin);
            var accessControl = ContentPartyRatingRoom.Create(Guid.NewGuid(), creator);

            var newUser = new Rater(Guid.NewGuid(), RoleType.Default);
            accessControl.InviteUser(newUser, creator.Id);

            var userInvitedEvent = accessControl.DomainEvents.OfType<RaterInvitedDomainEvent>().Single();
            Assert.Equal(newUser, userInvitedEvent.User);
            Assert.Equal(accessControl.Id, userInvitedEvent.RoomId);
        }

        [Theory]
        [InlineData(RoleType.Default)]
        [InlineData(RoleType.Mock)]
        public void InviteUser_ForbiddenInviteRole_ThrowForbiddenRoomOperationException(RoleType forbiddenInviteRole)
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin);
            var inviteInitiator = new Rater(Guid.NewGuid(), forbiddenInviteRole);
            var accessControl = ContentPartyRatingRoom.Create(Guid.NewGuid(), creator, [inviteInitiator]);
            
            var newUser = new Rater(Guid.NewGuid(), RoleType.Default);
            
            Assert.Throws<ForbiddenRoomOperationException>(()=> accessControl.InviteUser(newUser, inviteInitiator.Id));
            
        }
        [Fact]
        public void KickUser_ByAdmin_AddUserKickedEvent()
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin);
            var userForKick = new Rater(Guid.NewGuid(), RoleType.Default);
            var accessControl = ContentPartyRatingRoom.Create(Guid.NewGuid(), creator, [userForKick]);

            accessControl.KickRater(userForKick.Id, creator.Id);

            var userKickedEvent = accessControl.DomainEvents.OfType<RaterKickedDomainEvent>().Single();
            Assert.Equal(userForKick, userKickedEvent.KickedUser);
            Assert.Equal(accessControl.Id, userKickedEvent.RoomId);
        }
        [Theory]
        [InlineData(RoleType.Default)]
        [InlineData(RoleType.Mock)]
        public void KickUser_ForbiddenKickRole_ThrowForbiddenRoomOperationException(RoleType forbiddenInviteRole)
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin);
            var kickInitiator = new Rater(Guid.NewGuid(), forbiddenInviteRole);
            var kickTarget = new Rater(Guid.NewGuid(), RoleType.Default);
            var accessControl = ContentPartyRatingRoom.Create(Guid.NewGuid(), creator, [kickInitiator, kickTarget]);

            Assert.Throws<ForbiddenRoomOperationException>(() => accessControl.KickRater(kickTarget.Id, kickInitiator.Id));
        }

        [Fact]
        public void KickUser_ContentEstimated_ThrowInvalidRoomStageOperationException()
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin);
            var userForKick = new Rater(Guid.NewGuid(), RoleType.Default);
            var accessControl = ContentPartyRatingRoom.Create(Guid.NewGuid(), creator, [userForKick]);

            accessControl.StartControlContentEstimatedRoom();

            Assert.Throws<InvalidRoomStageOperationException>(() => accessControl.KickRater(userForKick.Id, creator.Id));
        }
        [Fact]
        public void RequestUserAccessInformation_FullAccess_ShouldCanEditAndViewRating()
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin);
            var accessControl = ContentPartyRatingRoom.Create(Guid.NewGuid(), creator);

            var accessInformation = accessControl.RequestAccessInformation(creator.Id);

            Assert.True(accessInformation.CanInviteUsers);
            Assert.True(accessInformation.CanViewRoom);
            Assert.True(accessInformation.CanRate);
            Assert.True(accessInformation.CanKickUsers);
        }
        [Fact]
        public void RequestUserAccessInformation_UnknownUserId_NoPermission()
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin);
            var accessControl = ContentPartyRatingRoom.Create(Guid.NewGuid(), creator);
            var unknownUserId = Guid.NewGuid();

            var accessInformation = accessControl.RequestAccessInformation(unknownUserId);

            Assert.False(accessInformation.CanInviteUsers);
            Assert.False(accessInformation.CanViewRoom);
            Assert.False(accessInformation.CanRate);
            Assert.False(accessInformation.CanKickUsers);
        }
        [Fact]
        public void RequestUserAccessInformation_DefaultUser_LimitedAccess()
        {
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin);
            var defaultUser = new Rater(Guid.NewGuid(), RoleType.Default);
            var accessControl = ContentPartyRatingRoom.Create(Guid.NewGuid(), creator, [defaultUser]);

            var accessInformation = accessControl.RequestAccessInformation(defaultUser.Id);

            Assert.False(accessInformation.CanInviteUsers);
            Assert.True(accessInformation.CanViewRoom);
            Assert.True(accessInformation.CanRate);
            Assert.False(accessInformation.CanKickUsers);
        }
    }
}
