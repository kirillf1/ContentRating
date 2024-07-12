using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using Microsoft.AspNetCore.Authorization;

namespace ContentRatingAPI.Infrastructure.Authorization
{
    public class ContentPartyEstimationRoomRaterAccessHandler : AuthorizationHandler<ContentPartyEstimationRoomRaterAccessRequirement>
    {
        private readonly IContentPartyEstimationRoomRepository contentPartyEstimationRoomRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ContentPartyEstimationRoomRaterAccessHandler(IContentPartyEstimationRoomRepository contentPartyEstimationRoomRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.contentPartyEstimationRoomRepository = contentPartyEstimationRoomRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, ContentPartyEstimationRoomRaterAccessRequirement requirement)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var roomId = httpContext?.TryGetRoomIdFromHttpContext();
            if (!roomId.HasValue || !Guid.TryParse(httpContext?.User.GetUserId(), out var userId))
            {
                context.Fail();
                return;
            }
            if (!await requirement.Pass(contentPartyEstimationRoomRepository, userId, roomId.Value))
            {
                context.Fail();
                return;
            }
            context.Succeed(requirement);

        }
    }
}
