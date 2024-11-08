// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate
{
    public class Rater : Entity
    {
        public Rater(Guid id, RoleType role, string name)
        {
            Id = id;
            Role = role;
            Name = name;
        }

        public RoleType Role { get; private set; }
        public string Name { get; }
    }
}
