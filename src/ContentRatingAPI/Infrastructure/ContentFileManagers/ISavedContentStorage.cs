// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRatingAPI.Application.ContentFileManager;

namespace ContentRatingAPI.Infrastructure.ContentFileManagers
{
    public interface ISavedContentStorage
    {
        Task<SavedContentFileInfo?> GetSavedContent(Guid Id);
        Task<IEnumerable<SavedContentFileInfo>> GetOldCheckedOrUncheckedContent(TimeSpan checkInterval);
        Task Update(SavedContentFileInfo file);
        Task DeleteSavedContent(Guid Id);
        Task Add(SavedContentFileInfo file);
    }
}
