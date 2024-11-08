// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRatingAPI.Application.ContentPartyRating.EstimateContent;
using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ContentRatingAPI.Hubs
{
    [Authorize]
    public class ContentPartyEstimationHub : Hub
    {
        private readonly IUserInfoService userInfoService;
        private readonly IMediator mediator;
        private readonly IContentPartyEstimationRoomRepository estimationRoomRepository;

        public ContentPartyEstimationHub(
            IUserInfoService userInfoService,
            IMediator mediator,
            IContentPartyEstimationRoomRepository estimationRoomRepository
        )
        {
            this.userInfoService = userInfoService;
            this.mediator = mediator;
            this.estimationRoomRepository = estimationRoomRepository;
        }

        public async Task JoinEstimationRoom(Guid roomId)
        {
            var userInfo = userInfoService.TryGetUserInfo() ?? throw new HubException("Unknown user info");
            if (!await estimationRoomRepository.HasRaterInRoom(roomId, userInfo.Id))
            {
                throw new HubException("Forbidden to connect to this room");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        public async Task ExitEstimationRoom(Guid roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        public async Task EstimateContent(Guid contentRatingId, EstimateContentRequest request)
        {
            var userInfo = userInfoService.TryGetUserInfo() ?? throw new HubException("Unknown user info");

            await mediator.Send(new EstimateContentCommand(contentRatingId, userInfo.Id, request.RaterForChangeScoreId, request.NewScore));
        }
    }
}
