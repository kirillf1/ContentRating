using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization;
using System.Reflection;

namespace ContentRatingAPI.Infrastructure.Data.MapConvensions
{
    public class ReadOnlyMemberFinderConvention : ConventionBase, IClassMapConvention
    {
        public void Apply(BsonClassMap classMap)
        {
            var readOnlyProperties = classMap.ClassType.GetTypeInfo()
                .GetProperties()
                .Where(p => p.CanRead && !p.CanWrite)
                .ToList();

            readOnlyProperties.ForEach(p => classMap.MapProperty(p.Name));
        }
    }
}
