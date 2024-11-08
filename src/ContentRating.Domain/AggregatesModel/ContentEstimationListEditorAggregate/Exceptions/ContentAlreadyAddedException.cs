// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Exceptions
{
    public class ContentAlreadyAddedException(string? message, Content foundContent) : Exception(message)
    {
        public Content FoundContent { get; } = foundContent;
    }
}
