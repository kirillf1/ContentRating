// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.Shared.Content;

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate
{
    public class ContentData : ValueObject
    {
        public ContentData(Guid id, string newName, string newPath, ContentType newContentType)
        {
            Id = id;
            Name = newName;
            Path = newPath;
            Type = newContentType;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Path { get; }
        public ContentType Type { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Path;
            yield return Type;
        }
    }
}
