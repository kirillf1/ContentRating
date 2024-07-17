using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ContentRatingAPI.Infrastructure.Hubs
{
    [Authorize]
    public class ContentPartyEstimationHub : Hub
    {
        public ContentPartyEstimationHub()
        {
            
        }
        public async Task Test()
        {
            await Clients.Caller.SendAsync("response");
        }
    }
}
