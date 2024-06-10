using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events;
using System.Data;
using ContentPartyEstimationRoomAggreagete = ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.ContentPartyEstimationRoom;

namespace ContentRatingAPI.Application.ContentRoomEditor.CompeteContentEditing
{
    public class ContentListCreatedDomainHandler : INotificationHandler<ContentListCreatedDomainEvent>
    {
        private readonly IContentPartyEstimationRoomRepository partyEstimationRoomRepository;
        private readonly ContentPartyRatingService contentPartyRatingService;

        public ContentListCreatedDomainHandler(IContentPartyEstimationRoomRepository partyEstimationRoomRepository, ContentPartyRatingService contentPartyRatingService)
        {
            this.partyEstimationRoomRepository = partyEstimationRoomRepository;
            this.contentPartyRatingService = contentPartyRatingService;
        }



        public async Task Handle(ContentListCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await CreatePartyEstimationRoom(notification);
        }
        private async Task CreatePartyEstimationRoom(ContentListCreatedDomainEvent @event)
        {
            var raters = @event.InvitedEditors.Select(MapEditorToRater).ToList();
            var roomCreator = MapEditorToRater(@event.Creator);
            var partyEstimationRoom = ContentPartyEstimationRoomAggreagete.Create(@event.RoomId, roomCreator, raters);

            partyEstimationRoomRepository.Add(partyEstimationRoom);
            await partyEstimationRoomRepository.UnitOfWork.SaveChangesAsync();

        }
        private static Rater MapEditorToRater(Editor editor)
        {
            return new Rater(editor.Id, RoleType.Admin, editor.Name);
        }
        private async Task CreateContentRating(ContentListCreatedDomainEvent @event)
        {

        }
        //private static ContentRater MapEditorToRater(Editor editor)
        //{
        //    return new Rater(editor.Id, RoleType.Admin, editor.Name);
        //}
    }
}
