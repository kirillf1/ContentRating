using ContentRatingAPI.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace ContentRatingAPI.Infrastructure.Hubs
{
    [Authorize]
    public class ContentPartyEstimationHub : Hub
    {
        private readonly IUserInfoService userInfoService;

        public ContentPartyEstimationHub(IUserInfoService userInfoService)
        {
            this.userInfoService = userInfoService;
        }
        public async Task Test()
        {
            var t =  userInfoService.TryGetUserInfo();
            await Clients.Caller.SendAsync("ReceiveMessage", "hi");
        }
    }
}
