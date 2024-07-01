using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using Microsoft.AspNetCore.Authorization;

namespace ContentRatingAPI.Infrastructure.Authorization
{
    public class ContentRoomEditorUserAccessHandler : AuthorizationHandler<ContentRoomEditorUserAccessRequirement>
    {
        private readonly IContentEditorRoomRepository editorRoomRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ContentRoomEditorUserAccessHandler(IContentEditorRoomRepository editorRoomRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.editorRoomRepository = editorRoomRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ContentRoomEditorUserAccessRequirement requirement)
        {
            try
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
            catch (Exception ex)
            {
                context.Fail();
            }
        }
    }
}
