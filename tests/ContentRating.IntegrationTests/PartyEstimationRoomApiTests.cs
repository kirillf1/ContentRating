using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.IntegrationTests.DataHelpers;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.ChangeRatingRange;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.InviteRater;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContentRating.IntegrationTests
{
    [Collection("ContentRating")]
    public class PartyEstimationRoomApiTests
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly Guid _userId;
        public PartyEstimationRoomApiTests(ContentRatingApiFixture fixture)
        {
            _userId = fixture.UserId;
            _webApplicationFactory = fixture;
            _httpClient = _webApplicationFactory.CreateDefaultClient();
            _serviceProvider = fixture.GetServiceProvider();
        }

        [Fact]
        public async Task Get_PartyEstimationRoom_Success()
        {
            var room = await CreatePartyEstimationRoom();

            var response = await _httpClient.GetAsync($"api/content-party-estimation-room/{room.Id}");
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
        [Fact]
        public async Task Get_NotRelatedUserForPartyEstimationRoom_Failed()
        {
            var unknownId = Guid.NewGuid();

            var response = await _httpClient.GetAsync($"api/content-party-estimation-room/{unknownId}");

            Assert.False(response.IsSuccessStatusCode);
        }
        [Fact]
        public async Task Get_PartyEstimationRooms_Success()
        {
            await CreatePartyEstimationRoom();

            var response = await _httpClient.GetAsync($"api/content-party-estimation-room");
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Delete_UnusedContent_Success()
        {
            var room = await CreatePartyEstimationRoom();

            var response = await _httpClient.DeleteAsync($"api/content-party-estimation-room/{room.Id}/content/{room.ContentForEstimation.First().Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        }
        [Fact]
        public async Task Delete_UnknownUnusedContent_NotFound()
        {
            var room = await CreatePartyEstimationRoom();
            var unknownContentId = Guid.NewGuid();

            var response = await _httpClient.DeleteAsync($"api/content-party-estimation-room/{room.Id}/content/{unknownContentId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task Delete_RaterFromRoom_Success()
        {
            var room = await CreatePartyEstimationRoom();
            var raterForKickId = room.Raters.First(c => c.Id != room.RoomCreator.Id).Id;

            var response = await _httpClient.DeleteAsync($"api/content-party-estimation-room/{room.Id}/rater/{raterForKickId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        [Fact]
        public async Task Delete_UnknownRaterFromRoom_Failed()
        {
            var room = await CreatePartyEstimationRoom();
            var unknownRater = Guid.NewGuid();

            var response = await _httpClient.DeleteAsync($"api/content-party-estimation-room/{room.Id}/rater/{unknownRater}");

            Assert.False(response.IsSuccessStatusCode);
        }
        [Fact]
        public async Task Post_CreatePartyEstimationRoom_Success()
        {
            var request = await CreatePartyEstimationRoomRequestBody();
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var roomId = Guid.NewGuid();

            var response = await _httpClient.PostAsync($"api/content-party-estimation-room/{roomId}", requestContent);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        [Fact]
        public async Task Post_InvalidCreatePartyEstimationRoom_BadRequest()
        {
            var request = await CreatePartyEstimationRoomRequestBody();
            request.MaxRating = 0;
            request.MinRating = 1;
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var roomId = Guid.NewGuid();

            var response = await _httpClient.PostAsync($"api/content-party-estimation-room/{roomId}", requestContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task Post_InvalidContentListForCreatePartyEstimationRoom_Failed()
        {
            var invalidContentListId = Guid.NewGuid();
            var request = await CreatePartyEstimationRoomRequestBody();
            request.ContentListId = invalidContentListId;
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var roomId = Guid.NewGuid();

            var response = await _httpClient.PostAsync($"api/content-party-estimation-room/{roomId}", requestContent);
            var s = await response.Content.ReadAsStringAsync();

            Assert.False(response.IsSuccessStatusCode);
        }
        [Fact]
        public async Task Post_InviteRater_Success()
        {
            var roomId = Guid.NewGuid();
            var request = await CreateInviteRaterRequest(roomId);
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            var raterId = Guid.NewGuid();
            var requestContent = new StringContent(JsonSerializer.Serialize(request, jsonOptions), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"api/content-party-estimation-room/{roomId}/rater/{raterId}", requestContent);
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        [Fact]
        public async Task Post_InviteRaterSameRaterId_BadRequest()
        {
            var roomId = Guid.NewGuid();
            var request = await CreateInviteRaterRequest(roomId);
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            var requestContent = new StringContent(JsonSerializer.Serialize(request, jsonOptions), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"api/content-party-estimation-room/{roomId}/rater/{_userId}", requestContent);
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task Put_ChangeRatingRange_Success()
        {
            var roomId = Guid.NewGuid();
            var request = await CreateChangeRatingRangeRequest(roomId);
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/content-party-estimation-room/{roomId}/rating-range", requestContent);
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Put_ChangeInvalidRatingRange_BadRequest()
        {
            var roomId = Guid.NewGuid();
            var request = await CreateChangeRatingRangeRequest(roomId);
            request.MinRating = request.MaxRating + 1;
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/content-party-estimation-room/{roomId}/rating-range", requestContent);
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task Put_CompleteContentEstimation_Success()
        {
            var room = await CreatePartyEstimationRoom();
           
            var response = await _httpClient.PutAsync($"api/content-party-estimation-room/{room.Id}/complete-estimation", null);
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
        [Fact]
        public async Task Put_CompleteContentEstimationUnknownRoom_Success()
        {
            var unknownRoomId = Guid.NewGuid();

            var response = await _httpClient.PutAsync($"api/content-party-estimation-room/{unknownRoomId}/complete-estimation", null);
            var s = await response.Content.ReadAsStringAsync();

            Assert.False(response.IsSuccessStatusCode);

        }
        private async Task<ChangeRatingRangeRequest> CreateChangeRatingRangeRequest(Guid roomId)
        {
            await CreatePartyEstimationRoom(roomId);
            return new ChangeRatingRangeRequest() { MaxRating = 10, MinRating = 0 };
        }
        private async Task<CreatePartyEstimationRoomRequest> CreatePartyEstimationRoomRequestBody()
        {
            var repository = _serviceProvider.GetRequiredService<IContentEditorRoomRepository>();
            var contentList = ContentRoomEditorGenerator.GenerateContentRoomEditor(_userId);
            repository.Add(contentList);
            await repository.UnitOfWork.SaveChangesAsync();

            return new CreatePartyEstimationRoomRequest()
            {
                ContentListId = contentList.Id,
                MaxRating = 10,
                MinRating = 0,
                RoomName = Guid.NewGuid().ToString()
            };

        }
        private async Task<InviteRaterRequest> CreateInviteRaterRequest(Guid roomId)
        {
            await CreatePartyEstimationRoom(roomId);
            return new InviteRaterRequest { RaterName = Guid.NewGuid().ToString(), RoleType = RoleType.Default };
        }
        private async Task<ContentPartyEstimationRoom> CreatePartyEstimationRoom(Guid? roomId = null)
        {
            var repository = _serviceProvider.GetRequiredService<IContentPartyEstimationRoomRepository>();
            var room = ContentPartyEstimationRoomGenerator.GeneratePartyEstimationRoom(_userId, roomId);
            repository.Add(room);
            await repository.UnitOfWork.SaveChangesAsync();
            return room;
        }
    }
}
