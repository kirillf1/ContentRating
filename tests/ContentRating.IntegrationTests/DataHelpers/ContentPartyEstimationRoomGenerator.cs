// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bogus;
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.Shared.Content;

namespace ContentRating.IntegrationTests.DataHelpers
{
    internal static class ContentPartyEstimationRoomGenerator
    {
        private static Random _random = new Random();
        private static Faker faker = new Faker();

        public static List<ContentPartyEstimationRoom> GenerateContentPartyEstimationRooms(
            int count,
            Guid? creatorId = null
        )
        {
            var rooms = new List<ContentPartyEstimationRoom>();

            for (int i = 0; i < count; i++)
            {
                ContentPartyEstimationRoom room = GeneratePartyEstimationRoom(
                    creatorId
                );
                rooms.Add(room);
            }

            return rooms;
        }

        public static ContentPartyEstimationRoom GeneratePartyEstimationRoom(
            Guid? creatorId = null,
            Guid? roomId = null
        )
        {
            var id = roomId ?? Guid.NewGuid();

            var creator = new Rater(
                creatorId ?? Guid.NewGuid(),
                RoleType.Admin,
                faker.Person.UserName
            );
            var contentList = GenerateRandomContentForEstimationList(
                _random.Next(1, 10)
            );
            var name = $"Sample Room Name {_random.Next(1000, 9999)}";
            var ratingRange = new RatingRange(
                Rating.DefaultMaxRating,
                Rating.DefaultMinRating
            );
            var otherInvitedRaters = GenerateRandomRaters(_random.Next(1, 5));

            var room = ContentPartyEstimationRoom.Create(
                id,
                creator,
                contentList,
                name,
                ratingRange,
                otherInvitedRaters
            );
            return room;
        }

        private static List<ContentForEstimation> GenerateRandomContentForEstimationList(
            int count
        )
        {
            var contentList = new List<ContentForEstimation>();
            var contentTypes = Enum.GetValues(typeof(ContentType));
            for (int i = 0; i < count; i++)
            {
                var contentType = (ContentType)
                    contentTypes.GetValue(_random.Next(0, contentTypes.Length));
                var content = new ContentForEstimation(
                    Guid.NewGuid(),
                    Guid.NewGuid().ToString(),
                    faker.Internet.Url(),
                    contentType
                );
                contentList.Add(content);
            }

            return contentList;
        }

        private static List<Rater> GenerateRandomRaters(int count)
        {
            var raters = new List<Rater>();

            for (int i = 0; i < count; i++)
            {
                var name = faker.Lorem.Sentence(3, 3);
                var rater = new Rater(Guid.NewGuid(), RoleType.Default, name);
                raters.Add(rater);
            }

            return raters;
        }
    }
}
