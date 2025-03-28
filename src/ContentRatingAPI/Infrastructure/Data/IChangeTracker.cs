// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.Shared;

namespace ContentRatingAPI.Infrastructure.Data
{
    public interface IChangeTracker
    {
        void TrackEntity(Entity entity);
        IEnumerable<Entity> GetTrackedEntities();
        void StopTrackEntity(Entity entity);
    }
}
