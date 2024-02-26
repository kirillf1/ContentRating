using ContentRating.Domain.Shared;

namespace ContentRatingAPI.Infrastructure.Data
{
    public class InMemoryChangeTracker : IChangeTracker
    {
        public InMemoryChangeTracker()
        {
            _trackedEntities = new List<Entity>();
        }
        private List<Entity> _trackedEntities; 
        public IEnumerable<Entity> GetTrackedEntities()
        {
            return _trackedEntities;
        }

        public void StopTrackEntity(Entity entity)
        {
            _trackedEntities.Remove(entity);
        }

        public void TrackEntity(Entity entity)
        {
            if (!_trackedEntities.Contains(entity))
                _trackedEntities.Add(entity);
        }
    }
}
