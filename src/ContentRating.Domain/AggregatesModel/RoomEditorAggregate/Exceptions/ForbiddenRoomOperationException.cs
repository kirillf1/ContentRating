namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Exceptions
{
    public class ForbiddenRoomOperationException(string message) : Exception(message)
    {
    }
}
