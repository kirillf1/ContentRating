using Bogus;
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.Shared.Content;

namespace ContentRating.IntegrationTests.DataHelpers
{
    internal static class ContentPartyEstimationGenerator
    {
        private static Random _random = new Random();
        private static Faker faker = new Faker();
        public static List<ContentPartyEstimationRoom> GenerateContentPartyEstimationRooms(int count, Guid? creatorId = null)
        {
            var rooms = new List<ContentPartyEstimationRoom>();

            for (int i = 0; i < count; i++)
            {
                ContentPartyEstimationRoom room = GeneratePartyEstimationRoom(creatorId);
                rooms.Add(room);
            }

            return rooms;
        }

        public static ContentPartyEstimationRoom GeneratePartyEstimationRoom(Guid? creatorId)
        {
            var id = creatorId ?? Guid.NewGuid();
            var creator = new Rater(Guid.NewGuid(), RoleType.Admin, faker.Person.UserName);
            var contentList = GenerateRandomContentForEstimationList(_random.Next(1, 10));
            var name = $"Sample Room Name {_random.Next(1000, 9999)}";
            var ratingRange = new RatingRange(Rating.DefaultMinRating, Rating.DefaultMaxRating);
            var otherInvitedRaters = GenerateRandomRaters(_random.Next(1, 5));

            var room = ContentPartyEstimationRoom.Create(id, creator, contentList, name, ratingRange, otherInvitedRaters);
            return room;
        }

        private static List<ContentForEstimation> GenerateRandomContentForEstimationList(int count)
        {
            var contentList = new List<ContentForEstimation>();
            var contentTypes = Enum.GetValues(typeof(ContentType));
            for (int i = 0; i < count; i++)
            {
                var contentType = (ContentType)contentTypes.GetValue(_random.Next(0, contentTypes.Length));
                var content = new ContentForEstimation(Guid.NewGuid(),Guid.NewGuid().ToString(), faker.Internet.Url(), contentType);
                contentList.Add(content);
            }

            return contentList;
        }

        private static List<Rater> GenerateRandomRaters(int count)
        {
            var raters = new List<Rater>();

            for (int i = 0; i < count; i++)
            {
                var rater = new Rater(Guid.NewGuid(), RoleType.Default, faker.Person.UserName);
                raters.Add(rater);
            }

            return raters;
        }
    }
}
