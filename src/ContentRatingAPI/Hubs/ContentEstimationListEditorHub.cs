using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ContentRatingAPI.Hubs
{
    public class ContentEstimationListEditorHub : Hub
    {
        private readonly IUserInfoService userInfoService;
        private readonly IContentEstimationListEditorRepository estimationListEditorRepository;

        public ContentEstimationListEditorHub(IUserInfoService userInfoService, IContentEstimationListEditorRepository estimationListEditorRepository)
        {
            this.userInfoService = userInfoService;
            this.estimationListEditorRepository = estimationListEditorRepository;
        }
        public async Task JoinContentEditing(Guid contentListId)
        {
            var userInfo = userInfoService.TryGetUserInfo() ?? throw new HubException("Unknown user info");
            if (!await estimationListEditorRepository.HasEditorInContentEstimationList(contentListId, userInfo.Id))
                throw new HubException("Forbidden to connect to this content list editor");

            await Groups.AddToGroupAsync(Context.ConnectionId, contentListId.ToString());
        }
        public async Task ExitContentEditing(Guid contentListId)
        { 
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, contentListId.ToString());
        }
    }
}
