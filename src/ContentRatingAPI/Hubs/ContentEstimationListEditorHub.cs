// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ContentRatingAPI.Hubs
{
    public class ContentEstimationListEditorHub : Hub
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IContentEstimationListEditorRepository _estimationListEditorRepository;

        public ContentEstimationListEditorHub(IUserInfoService userInfoService, IContentEstimationListEditorRepository estimationListEditorRepository)
        {
            this._userInfoService = userInfoService;
            this._estimationListEditorRepository = estimationListEditorRepository;
        }

        public async Task JoinContentEditing(Guid contentListId)
        {
            var userInfo = _userInfoService.TryGetUserInfo() ?? throw new HubException("Unknown user info");
            if (!await _estimationListEditorRepository.HasEditorInContentEstimationList(contentListId, userInfo.Id))
            {
                throw new HubException("Forbidden to connect to this content list editor");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, contentListId.ToString());
        }

        public async Task ExitContentEditing(Guid contentListId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, contentListId.ToString());
        }
    }
}
