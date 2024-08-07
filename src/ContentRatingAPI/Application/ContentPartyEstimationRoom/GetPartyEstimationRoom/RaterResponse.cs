namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoom
{
    public class RaterResponse
    {
        public RaterResponse(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
   
}
