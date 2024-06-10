using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization;
using System.Linq.Expressions;

namespace ContentRatingAPI.Infrastructure.Data.MapConvensions
{
    public static class DictionaryRepresentationMapHelper
    {
        public static BsonClassMap<T> SetDictionaryRepresentation<T, TMember>(this BsonClassMap<T> classMap, Expression<Func<T, TMember>> memberLambda, DictionaryRepresentation representation)
        {
            var memberMap = classMap.GetMemberMap(memberLambda);
            return SetDictionaryRepresentation(classMap, representation, memberMap);
        }
        public static BsonClassMap<T> SetDictionaryRepresentation<T>(this BsonClassMap<T> classMap, string memberName, DictionaryRepresentation representation)
        {
            var memberMap = classMap.GetMemberMap(memberName);
            return SetDictionaryRepresentation(classMap, representation, memberMap);
        }

        private static BsonClassMap<T> SetDictionaryRepresentation<T>(BsonClassMap<T> classMap, DictionaryRepresentation representation, BsonMemberMap memberMap)
        {
            var serializer = memberMap.GetSerializer();
            
            if (serializer is IDictionaryRepresentationConfigurable dictionaryRepresentationSerializer)
                serializer = dictionaryRepresentationSerializer.WithDictionaryRepresentation(representation);
            memberMap.SetSerializer(serializer);
            return classMap;
        }
    }
}
