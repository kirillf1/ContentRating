// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Infrastructure.Data
{
    public class MongoDBOptions
    {
        public const string Position = "MongoDB";

        public string Connection { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
        public string ContentEstimationListEditorCollectionName { get; set; } = String.Empty;
        public string ContentPartyEstimationRoomCollectionName { get; set; } = String.Empty;
        public string ContentPartyRatingCollectionName { get; set; } = String.Empty;
        public string SavedContentFileCollectionName { get; set; } = String.Empty;
    }
}
