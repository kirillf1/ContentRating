// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;

namespace ContentRatingAPI.Infrastructure.AggregateIntegration.ContentPartyRating
{
    public class ContentRaterTranslator
    {
        public ContentRater Translate(Rater rater)
        {
            var role = rater.Role;
            var raterType = role switch
            {
                RoleType.Admin => RaterType.Admin,
                RoleType.Default => RaterType.Default,
                RoleType.Mock => RaterType.Mock,
                _ => throw new NotImplementedException(),
            };
            return new ContentRater(rater.Id, raterType);
        }
    }
}
