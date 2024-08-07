namespace ContentRatingAPI.Application.Identity.GetAllUsers
{
    public class UserResponse
    {
        public UserResponse(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
}
