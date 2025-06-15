using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Web.Contracts.ContentPartyEstimationRoom;
using ContentRating.Web.Contracts.Identity;

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

        public UserService(HttpClient httpClient, IContentPartyEstimationService estimationService)
        {
            _httpClient = httpClient;
            _estimationService = estimationService;
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
                Console.WriteLine($"Ошибка при получении пользователей: {ex.Message}");
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
                Console.WriteLine($"Ошибка при приглашении пользователя: {ex.Message}");
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
                Console.WriteLine($"Ошибка при приглашении mock пользователя: {ex.Message}");
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
