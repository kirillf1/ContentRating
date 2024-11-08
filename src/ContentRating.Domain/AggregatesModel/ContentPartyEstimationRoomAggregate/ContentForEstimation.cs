// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.Shared.Content;

namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate
{
    public class ContentForEstimation : Entity
    {
        public ContentForEstimation(Guid id, string name, string url, ContentType contentType)
        {
            Id = id;
            Name = name;
            Url = url;
            ContentType = contentType;
        }

        public string Name { get; private set; }
        public string Url { get; private set; }
        public ContentType ContentType { get; private set; }
    }
}
