// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;

namespace ContentRatingAPI.Infrastructure.Data.MapConvensions
{
    public static class DictionaryRepresentationMapHelper
    {
        public static BsonClassMap<T> SetDictionaryRepresentation<T, TMember>(
            this BsonClassMap<T> classMap,
            Expression<Func<T, TMember>> memberLambda,
            DictionaryRepresentation representation
        )
        {
            var memberMap = classMap.GetMemberMap(memberLambda);
            return SetDictionaryRepresentation(classMap, representation, memberMap);
        }

        public static BsonClassMap<T> SetDictionaryRepresentation<T>(
            this BsonClassMap<T> classMap,
            string memberName,
            DictionaryRepresentation representation
        )
        {
            var memberMap = classMap.GetMemberMap(memberName);
            return SetDictionaryRepresentation(classMap, representation, memberMap);
        }

        private static BsonClassMap<T> SetDictionaryRepresentation<T>(
            BsonClassMap<T> classMap,
            DictionaryRepresentation representation,
            BsonMemberMap memberMap
        )
        {
            var serializer = memberMap.GetSerializer();

            if (serializer is IDictionaryRepresentationConfigurable dictionaryRepresentationSerializer)
            {
                serializer = dictionaryRepresentationSerializer.WithDictionaryRepresentation(representation);
            }

            memberMap.SetSerializer(serializer);
            return classMap;
        }
    }
}
