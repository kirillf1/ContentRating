using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.IntegrationTests.DataHelpers;
using ContentRating.IntegrationTests.Fixtures;
using ContentRatingAPI.Application.ContentPartyRating.EstimateContent;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ContentRating.IntegrationTests
{
    [Collection("ContentRating")]
    public class PartyRatingApiTests
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly Guid _userId;
        public PartyRatingApiTests(ContentRatingApiFixture fixture)
        {
            _userId = fixture.UserId;
            _webApplicationFactory = fixture;
            _httpClient = _webApplicationFactory.CreateDefaultClient();
            _serviceProvider = fixture.GetServiceProvider();
        }
        [Fact]
        public async Task Get_ContentPartyRating_Success()
        {
            var rating = await CreateContentPartyRating();

            var response = await _httpClient.GetAsync($"api/content-party-rating/{rating.Id}");
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
        [Fact]
        public async Task Get_UnknownContentPartyRating_NotFound()
        {
            var unknownRatingId = Guid.NewGuid();

            var response = await _httpClient.GetAsync($"api/content-party-rating/{unknownRatingId}");
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task Put_EstimateContent_Success()
        {
            var requestParams = await CreateEstimateContentRequest();
            var request = requestParams.Item1;
            var ratingId = requestParams.Item2;
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/content-party-rating/{ratingId}", requestContent);
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
        [Fact]
        public async Task Put_EstimateContentInvalidRating_BadRequest()
        {
            var requestParams = await CreateEstimateContentRequest();
            var request = requestParams.Item1;
            request.NewScore = -1;
            var ratingId = requestParams.Item2;
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/content-party-rating/{ratingId}", requestContent);
            var s = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }
        private async Task<(EstimateContentRequest, Guid)> CreateEstimateContentRequest()
        {
            var rating = await CreateContentPartyRating();
            return (new EstimateContentRequest() { RaterForChangeScoreId = _userId, NewScore = 4 }, rating.Id);
        }
        private async Task<ContentPartyRating> CreateContentPartyRating()
        {
            var partyEstimationRepository = _serviceProvider.GetRequiredService<IContentPartyEstimationRoomRepository>();
            var room = ContentPartyEstimationRoomGenerator.GeneratePartyEstimationRoom(_userId);
            partyEstimationRepository.Add(room);
            await partyEstimationRepository.UnitOfWork.SaveChangesAsync();
            var contentPartyRatingRepository = _serviceProvider.GetRequiredService<IContentPartyRatingRepository>();
            var ratings = await contentPartyRatingRepository.GetContentRatingsByRoom(room.Id);
            return ratings.First();
        }
    }
}
