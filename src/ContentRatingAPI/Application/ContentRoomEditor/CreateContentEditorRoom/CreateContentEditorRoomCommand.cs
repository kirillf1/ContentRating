namespace ContentRatingAPI.Application.ContentRoomEditor.CreateContentEditorRoom
{
    public record CreateContentEditorRoomCommand(Guid Id, Guid CreatorId, string CreatorName, string RoomName) : IRequest<Result<bool>>;
}
