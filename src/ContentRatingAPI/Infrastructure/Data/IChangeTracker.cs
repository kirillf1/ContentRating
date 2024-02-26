using ContentRating.Domain.Shared;

namespace ContentRatingAPI.Infrastructure.Data
{
    public interface IChangeTracker
    {
        void TrackEntity(Entity entity);
        IEnumerable<Entity> GetTrackedEntities();
        void StopTrackEntity(Entity entity);
    }
}
