// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate
{
    public class RatingRange : ValueObject
    {
        public RatingRange(Rating maxRating, Rating minRating)
        {
            if (minRating.Value > maxRating.Value)
            {
                throw new ArgumentException("Min rating can't be more than max rating");
            }

            MaxRating = maxRating;
            MinRating = minRating;
        }

        public RatingRange()
        {
            MaxRating = Rating.DefaultMaxRating;
            MinRating = Rating.DefaultMinRating;
        }

        public Rating MaxRating { get; }
        public Rating MinRating { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return MaxRating;
            yield return MinRating;
        }
    }
}
