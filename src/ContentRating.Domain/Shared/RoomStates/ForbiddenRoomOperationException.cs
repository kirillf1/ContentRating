namespace ContentRating.Domain.Shared.RoomStates
{
    public class ForbiddenRoomOperationException(string message) : Exception(message)
    {
    }
}
