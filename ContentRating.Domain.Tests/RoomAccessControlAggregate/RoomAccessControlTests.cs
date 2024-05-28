using ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate;
using ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate.Events;
using ContentRating.Domain.Shared.RoomStates;
using Xunit;

namespace ContentRating.Domain.Tests.RoomAccessControlAggregate
{
    public class RoomAccessControlTests
    {
        [Fact]
        public void InviteUser_ByAdmin_AddedUserInvitedEvent()
        {
            var creator = new User(Guid.NewGuid(), RoleType.Admin);
            var accessControl = RoomAccessControl.Create(Guid.NewGuid(), creator);

            var newUser = new User(Guid.NewGuid(), RoleType.Default);
            accessControl.InviteUser(newUser, creator.Id);

            var userInvitedEvent = accessControl.DomainEvents.OfType<UserInvitedDomainEvent>().Single();
            Assert.Equal(newUser, userInvitedEvent.User);
            Assert.Equal(accessControl.Id, userInvitedEvent.RoomId);
        }

        [Theory]
        [InlineData(RoleType.Default)]
        [InlineData(RoleType.Mock)]
        public void InviteUser_ForbiddenInviteRole_ThrowForbiddenRoomOperationException(RoleType forbiddenInviteRole)
        {
            var creator = new User(Guid.NewGuid(), RoleType.Admin);
            var inviteInitiator = new User(Guid.NewGuid(), forbiddenInviteRole);
            var accessControl = RoomAccessControl.Create(Guid.NewGuid(), creator, [inviteInitiator]);
            
            var newUser = new User(Guid.NewGuid(), RoleType.Default);
            
            Assert.Throws<ForbiddenRoomOperationException>(()=> accessControl.InviteUser(newUser, inviteInitiator.Id));
            
        }
        [Fact]
        public void KickUser_ByAdmin_AddUserKickedEvent()
        {
            var creator = new User(Guid.NewGuid(), RoleType.Admin);
            var userForKick = new User(Guid.NewGuid(), RoleType.Default);
            var accessControl = RoomAccessControl.Create(Guid.NewGuid(), creator, [userForKick]);

            accessControl.KickUser(userForKick.Id, creator.Id);

            var userKickedEvent = accessControl.DomainEvents.OfType<UserKickedDomainEvent>().Single();
            Assert.Equal(userForKick, userKickedEvent.KickedUser);
            Assert.Equal(accessControl.Id, userKickedEvent.RoomId);
        }
        [Theory]
        [InlineData(RoleType.Default)]
        [InlineData(RoleType.Mock)]
        public void KickUser_ForbiddenKickRole_ThrowForbiddenRoomOperationException(RoleType forbiddenInviteRole)
        {
            var creator = new User(Guid.NewGuid(), RoleType.Admin);
            var kickInitiator = new User(Guid.NewGuid(), forbiddenInviteRole);
            var kickTarget = new User(Guid.NewGuid(), RoleType.Default);
            var accessControl = RoomAccessControl.Create(Guid.NewGuid(), creator, [kickInitiator, kickTarget]);

            Assert.Throws<ForbiddenRoomOperationException>(() => accessControl.KickUser(kickTarget.Id, kickInitiator.Id));
        }

        [Fact]
        public void KickUser_AccessControlStopped_ThrowInvalidRoomStageOperationException()
        {
            var creator = new User(Guid.NewGuid(), RoleType.Admin);
            var userForKick = new User(Guid.NewGuid(), RoleType.Default);
            var accessControl = RoomAccessControl.Create(Guid.NewGuid(), creator, [userForKick]);

            accessControl.StopAccessControl();

            Assert.Throws<InvalidRoomStageOperationException>(() => accessControl.KickUser(userForKick.Id, creator.Id));
        }
    }
}
