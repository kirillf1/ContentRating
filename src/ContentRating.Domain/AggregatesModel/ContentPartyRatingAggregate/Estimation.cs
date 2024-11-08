﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class Estimation : ValueObject
    {
        public Estimation(ContentRater estimationInitiator, ContentRater raterForChangeScore, Score newScore)
        {
            ContentEstimationInitiator = estimationInitiator;
            RaterForChangeScore = raterForChangeScore;
            NewScore = newScore;
        }

        public ContentRater ContentEstimationInitiator { get; }
        public ContentRater RaterForChangeScore { get; }
        public Score NewScore { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ContentEstimationInitiator;
            yield return RaterForChangeScore;
            yield return NewScore;
        }
    }
}
