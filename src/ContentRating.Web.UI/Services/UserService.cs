using System.Text.Json;
using System.Text.Json.Serialization;
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Web.Contracts.ContentPartyEstimationRoom;
using ContentRating.Web.Contracts.Identity;
using Microsoft.Extensions.Logging;

namespace ContentRating.Web.UI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>?> GetAllUsersAsync();
        Task<bool> InviteUserToRoomAsync(
            Guid roomId,
            Guid userId,
            string userName,
            RoleType roleType = RoleType.Default
        );
        Task<bool> InviteMockUserToRoomAsync(Guid roomId, string mockUserName);
        UserResponse CreateMockUser(string name);
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IContentPartyEstimationService _estimationService;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly List<UserResponse> _mockUsers = new();
        private readonly ILogger<UserService> _logger;

        public UserService(
            HttpClient httpClient,
            IContentPartyEstimationService estimationService,
            ILogger<UserService> logger
        )
        {
            _httpClient = httpClient;
            _estimationService = estimationService;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<IEnumerable<UserResponse>?> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("accounts");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<IEnumerable<UserResponse>>(
                        jsonString,
                        _jsonOptions
                    );

                    // Добавляем mock пользователей к списку
                    if (users != null)
                    {
                        var allUsers = users.ToList();
                        allUsers.AddRange(_mockUsers);
                        return allUsers;
                    }
                }

                return _mockUsers.AsEnumerable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователей");
                return _mockUsers.AsEnumerable();
            }
        }

        public async Task<bool> InviteUserToRoomAsync(
            Guid roomId,
            Guid userId,
            string userName,
            RoleType roleType = RoleType.Default
        )
        {
            try
            {
                var inviteRequest = new InviteRaterRequest
                {
                    RaterId = userId,
                    RaterName = userName,
                    RoleType = roleType,
                };

                return await _estimationService.InviteRaterAsync(roomId, inviteRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при приглашении пользователя {UserName} в комнату {RoomId}", userName, roomId);
                return false;
            }
        }

        public async Task<bool> InviteMockUserToRoomAsync(Guid roomId, string mockUserName)
        {
            try
            {
                // Создаем mock пользователя
                var mockUser = CreateMockUser(mockUserName);

                // Приглашаем с ролью Mock
                return await InviteUserToRoomAsync(
                    roomId,
                    mockUser.Id,
                    mockUser.Name,
                    RoleType.Mock
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании и приглашении mock пользователя {MockUserName} в комнату {RoomId}", mockUserName, roomId);
                return false;
            }
        }

        public UserResponse CreateMockUser(string name)
        {
            var mockUser = new UserResponse(Guid.NewGuid(), $"Mock: {name}");

            // Добавляем в локальный список mock пользователей
            if (!_mockUsers.Any(u => u.Name == mockUser.Name))
            {
                _mockUsers.Add(mockUser);
            }

            return mockUser;
        }
    }
}
