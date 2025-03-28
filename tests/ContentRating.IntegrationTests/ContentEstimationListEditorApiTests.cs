// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentRating.IntegrationTests.DataHelpers;
using ContentRating.IntegrationTests.Fixtures;
using ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications;
using ContentRatingAPI.Application.ContentEstimationListEditor.CreateContentEstimationListEditor;
using ContentRatingAPI.Application.ContentEstimationListEditor.InviteEditor;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ContentRating.IntegrationTests
{
    [Collection("ContentRating")]
    public class ContentEstimationListEditorApiTests
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly Guid _userId;

        public ContentEstimationListEditorApiTests(
            ContentRatingApiFixture fixture
        )
        {
            _userId = ContentRatingApiFixture.UserId;
            _webApplicationFactory = fixture;
            _httpClient = _webApplicationFactory.CreateDefaultClient();
            _serviceProvider = fixture.GetServiceProvider();
        }

        [Fact]
        public async Task Post_ContentEstimationListEditor_Success()
        {
            var requestContent = CreateContentEstimationListEditorRequestBody();

            var response = await _httpClient.PostAsync(
                $"api/content-estimation-list-editor",
                requestContent
            );
            var r = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_ContentEstimationListEditorExistingList_Error()
        {
            var contentList = await CreateContentEstimationListEditor();
            var requestContent = CreateContentEstimationListEditorRequestBody(
                contentList.Id
            );

            var response = await _httpClient.PostAsync(
                $"api/content-estimation-list-editor",
                requestContent
            );
            var r = await response.Content.ReadAsStringAsync();

            Assert.Equal(
                HttpStatusCode.InternalServerError,
                response.StatusCode
            );
        }

        [Fact]
        public async Task Post_InviteEditor_Success()
        {
            var contentList = await CreateContentEstimationListEditor();
            var request = new InviteEditorRequest
            {
                EditorId = Guid.NewGuid(),
                EditorName = Guid.NewGuid().ToString(),
            };
            var requestString = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(
                requestString,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"api/content-estimation-list-editor/{contentList.Id}/editor",
                stringContent
            );
            var r = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_CreateNewContent_Success()
        {
            var contentList = await CreateContentEstimationListEditor();
            var request = CreateContentRequestBody();

            var response = await _httpClient.PostAsync(
                $"api/content-estimation-list-editor/{contentList.Id}/content",
                request
            );

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateContent_Success()
        {
            var contentList = await CreateContentEstimationListEditor();
            var content = contentList.AddedContent.First();
            var request = new UpdateContentRequest
            {
                ContentType = Domain.Shared.Content.ContentType.Audio,
                Name = Guid.NewGuid().ToString(),
                Url = $"https://localhost/{Guid.NewGuid()}",
            };
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            var requestString = JsonSerializer.Serialize(request, jsonOptions);
            var stringContent = new StringContent(
                requestString,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync(
                $"api/content-estimation-list-editor/{contentList.Id}/content/{content.Id}",
                stringContent
            );
            var r = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_UpdateContentWithInvalidParams_BadRequest()
        {
            var contentList = await CreateContentEstimationListEditor();
            var content = contentList.AddedContent.First();
            var request = new UpdateContentRequest
            {
                ContentType = Domain.Shared.Content.ContentType.Audio,
                Name = string.Empty,
                Url = string.Empty,
            };
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            var requestString = JsonSerializer.Serialize(request, jsonOptions);
            var stringContent = new StringContent(
                requestString,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync(
                $"api/content-estimation-list-editor/{contentList.Id}/content/{content.Id}",
                stringContent
            );
            var r = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Content_Success()
        {
            var contentList = await CreateContentEstimationListEditor();
            var content = contentList.AddedContent.First();

            var response = await _httpClient.DeleteAsync(
                $"api/content-estimation-list-editor/{contentList.Id}/content/{content.Id}"
            );

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Editor_Success()
        {
            var contentList = await CreateContentEstimationListEditor();
            var editorId = contentList
                .InvitedEditors.First(c =>
                    c.Id != contentList.ContentListCreator.Id
                )
                .Id;

            var response = await _httpClient.DeleteAsync(
                $"api/content-estimation-list-editor/{contentList.Id}/editor/{editorId}"
            );

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Get_ContentListEditor_Success()
        {
            var contentList = await CreateContentEstimationListEditor();

            var response = await _httpClient.GetAsync(
                $"api/content-estimation-list-editor/{contentList.Id}"
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_ContentListEditors_Success()
        {
            var response = await _httpClient.GetAsync(
                $"api/content-estimation-list-editor"
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private StringContent CreateContentEstimationListEditorRequestBody(
            Guid? id = null
        )
        {
            var request = new CreateContentEstimationListEditorRequest()
            {
                Id = id ?? Guid.NewGuid(),
                RoomName = Guid.NewGuid().ToString(),
            };
            var requestString = JsonSerializer.Serialize(request);
            return new StringContent(
                requestString,
                Encoding.UTF8,
                "application/json"
            );
        }

        private StringContent CreateContentRequestBody(Guid? contentId = null)
        {
            var request = new CreateContentRequest()
            {
                Id = contentId ?? Guid.NewGuid(),
                ContentType = Domain.Shared.Content.ContentType.Audio,
                Name = Guid.NewGuid().ToString(),
                Url = $"https://localhost/{Guid.NewGuid()}",
            };
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            var requestString = JsonSerializer.Serialize(request, jsonOptions);
            return new StringContent(
                requestString,
                Encoding.UTF8,
                "application/json"
            );
        }

        private async Task<ContentEstimationListEditor> CreateContentEstimationListEditor()
        {
            var repository =
                _serviceProvider.GetRequiredService<IContentEstimationListEditorRepository>();
            var contentEditor =
                ContentEstimationListEditorGenerator.ContentEstimationListEditor(
                    _userId
                );
            repository.Add(contentEditor);
            await repository.UnitOfWork.SaveChangesAsync();
            return contentEditor;
        }
    }
}
