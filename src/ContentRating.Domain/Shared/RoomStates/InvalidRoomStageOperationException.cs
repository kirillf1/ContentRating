namespace ContentRating.Domain.Shared.RoomStates
{
    public class InvalidRoomStageOperationException(string message) : Exception(message)
    {
    }
}
