// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using Microsoft.AspNetCore.Authorization;

namespace ContentRatingAPI.Infrastructure.Authorization
{
    public class ContentEstimationListEditorUserAccessHandler : AuthorizationHandler<ContentEstimationListEditorUserAccessRequirement>
    {
        private readonly IContentEstimationListEditorRepository editorRoomRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ContentEstimationListEditorUserAccessHandler(
            IContentEstimationListEditorRepository editorRoomRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            this.editorRoomRepository = editorRoomRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ContentEstimationListEditorUserAccessRequirement requirement
        )
        {
            var httpContext = httpContextAccessor.HttpContext;
            var roomId = httpContext?.TryGetRoomIdFromHttpContext();
            if (!roomId.HasValue || !Guid.TryParse(httpContext?.User.GetUserId(), out var userId))
            {
                context.Fail();
                return;
            }
            if (!await requirement.Pass(editorRoomRepository, userId, roomId.Value))
            {
                context.Fail();
                return;
            }
            context.Succeed(requirement);
        }
    }
}
