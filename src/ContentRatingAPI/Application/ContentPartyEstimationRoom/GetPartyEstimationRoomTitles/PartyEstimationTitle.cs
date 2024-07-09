namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoomTitles
{
    public class PartyEstimationTitle
    {
        public PartyEstimationTitle(Guid id, string roomName, string roomCreatorName, int ratersCount, int contentCount, bool isEstimated)
        {
            Id = id;
            RoomName = roomName;
            RoomCreatorName = roomCreatorName;
            RatersCount = ratersCount;
            ContentCount = contentCount;
            IsEstimated = isEstimated;
        }

        public Guid Id { get; }
        public string RoomName { get; }
        public string RoomCreatorName { get; }
        public int RatersCount { get; }
        public int ContentCount { get; }
        public bool IsEstimated { get; }
    }
}
