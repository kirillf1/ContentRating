namespace ContentRatingAPI.Application.ContentRoomEditor.ChangeRatingRange
{
    public record ChangeRatingRangeCommand(Guid RoomId, Guid EditorId, double MinRating, double MaxRating) : IRequest;
}
