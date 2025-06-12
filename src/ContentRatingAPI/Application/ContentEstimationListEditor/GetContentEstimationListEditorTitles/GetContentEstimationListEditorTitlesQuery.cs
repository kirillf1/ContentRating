// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Web.Contracts.ContentEstimationListEditor;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditorTitles
{
    public record class GetContentEstimationListEditorTitlesQuery(Guid EditorId) : IRequest<Result<IEnumerable<ContentEstimationListEditorTitle>>>;
}
