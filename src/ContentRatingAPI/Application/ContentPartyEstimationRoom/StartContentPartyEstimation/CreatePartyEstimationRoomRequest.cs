namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation
{
    public class CreatePartyEstimationRoomRequest
    {
        public required Guid RoomId { get; set; } 
        public required Guid ContentListId { get; set; }
        public required string RoomName { get; set; }
        public required double MinRating { get; set; }
        public required double MaxRating { get; set; }
    }
}
