using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.IntegrationTests.DataHelpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace ContentRating.IntegrationTests
{
    public class PartyEstimationRoomApiTests : IClassFixture<ContentRatingApiFixture>
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly IContentEditorRoomRepository _repository;
        public PartyEstimationRoomApiTests(ContentRatingApiFixture fixture)
        {
            _webApplicationFactory = fixture;
            _httpClient = _webApplicationFactory.CreateDefaultClient();
             _repository = _webApplicationFactory.Services.GetRequiredService<IContentEditorRoomRepository>();
        }

        [Fact]
        public async Task GetRooms()
        {
            var room = ContentRoomEditorGenerator.CreateContentRoomEditor(ContentRatingApiFixture.UserId);
            _repository.Add(room);
            await _repository.UnitOfWork.SaveChangesAsync();

            var response = await _httpClient.GetAsync($"api/content-room-editor/{room.Id}");
            var s = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
    }
}
