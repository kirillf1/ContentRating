using ContentRating.Domain.AggregatesModel.ContentRatingAggregate;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate;
using ContentRatingAPI.Infrastructure.Data;
using ContentRatingAPI.Infrastructure.Data.MapConvensions;
using ContentRatingAPI.Infrastructure.Data.Repositories;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Moq;
using System;
using ContentRatingAggregate = ContentRating.Domain.AggregatesModel.ContentRatingAggregate.ContentRating;

namespace ContentRating.IntegrationTests
{
    public class TestEntity(Guid id, string name, string t)
    {

        public Guid Id { get; set; } = id;
        public string Name { get; set; } = name;
        public string T { get; set; } = t;

    }
    public class TestEntity1(Guid id, string name, string t)
    {

        public Guid Id { get; set; } = id;
        public string Name { get; set; } = name;
        public string T { get; set; } = t;

    }
    public class UnitTest1
    {

        [Fact]
        public void Test1()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var d = mongoClient.GetDatabase("MyTestDb");
            var c = d.GetCollection<TestEntity>("rooms");
            //b1cec36a-97d4-4b89-8baf-c18612f2454d
            var e = new TestEntity(Guid.NewGuid(), "test", "t");
            c.InsertOne(e);

        }
        [Fact]
        public void Test2()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var d = mongoClient.GetDatabase("MyTestDb");
            var c = d.GetCollection<ContentRoomEditor>("rooms");
            var e = c.Find(Builders<ContentRoomEditor>.Filter.Eq(c => c.Id, new Guid("50def115-72f9-4d08-aa7e-6d1c50d172e9"))).First();
            Console.WriteLine(e);
        }
        [Fact]
        public async Task Test3()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var d = mongoClient.GetDatabase("MyTestDb");
            var c = d.GetCollection<TestEntity>("rooms");
            using (var Session = await mongoClient.StartSessionAsync())
            {
                Session.StartTransaction();
                try
                {
                    var c1 = d.GetCollection<TestEntity1>("rooms1");
                    var e = new TestEntity(Guid.NewGuid(), "test", "t");
                    c.InsertOne(Session, e);
                    using (var session = await mongoClient.StartSessionAsync())
                    {
                        session.StartTransaction();
                        var newEntity = new TestEntity1(Guid.NewGuid(), "test1", "t");
                        c1.InsertOne(session, newEntity);
                        session.CommitTransaction();
                    }
                    Session.CommitTransaction();
                }

                catch (Exception ex)
                {
                    await Session.AbortTransactionAsync();
                }
            }
        }
        [Fact]
        public async Task Test4()
        {

            var conventionPack = new ConventionPack
            {
                new MapReadOnlyPropertiesConvention()
            };

            ConventionRegistry.Register("Conventions", conventionPack, _ => true);
            BsonClassMap.RegisterClassMap<ContentRoomEditor>(classMap =>
            {
                classMap.AutoMap();
            });
            var mediatr = new Mock<IMediator>();
            var changeTracker = new InMemoryChangeTracker();
            var mongoContext = new MongoContext(new MongoDBOptions { Connection = "mongodb://localhost:27017", DatabaseName = "MyTestDb" }, changeTracker, mediatr.Object);
            var rep = new RoomRepository(mongoContext, changeTracker);

            var id = Guid.NewGuid();
            var editor = new Editor(Guid.NewGuid(), "test");
            var newRoom = new ContentRoomEditor(id, editor, "testRoom");
            newRoom.CreateContent(editor, new ContentData(Guid.NewGuid(), "test", "test", ContentType.Image));
            rep.Add(newRoom);

            await rep.UnitOfWork.SaveChangesAsync();
            var room = await rep.GetRoom(id);
            await Console.Out.WriteLineAsync();
        }
        [Fact]
        public async Task Test5()
        {
            var conventionPack = new ConventionPack
            {
                new MapReadOnlyPropertiesConvention()
            };
            
            ConventionRegistry.Register("Conventions", conventionPack, _ => true);
                BsonClassMap.RegisterClassMap<ContentRatingAggregate>(classMap =>
            {
                classMap.AutoMap();
                classMap.MapField("_specification").SetElementName("Specification");
                classMap.MapField("_raters").SetElementName("Raters");
                classMap.UnmapProperty(c => c.Raters);
            

            });
            var mediatr = new Mock<IMediator>();
            var changeTracker = new InMemoryChangeTracker();
            var mongoContext = new MongoContext(new MongoDBOptions { Connection = "mongodb://localhost:27017", DatabaseName = "MyTestDb" }, changeTracker, mediatr.Object);
            var rep = new ContentRatingRepository(mongoContext, changeTracker);

            var id = Guid.NewGuid();
            var spec = new ContentRatingSpecification(new Score(0), new Score(10));
            var raters = new List<Rater> { new Rater(Guid.NewGuid(), RaterType.Owner, spec.MinScore), new Rater(Guid.NewGuid(), RaterType.Owner, spec.MinScore) };
            var rating = ContentRatingAggregate.Create(id, Guid.NewGuid(), raters, spec);
            rep.Add(rating);

            await rep.UnitOfWork.SaveChangesAsync();
            var r = await rep.GetContentRating(id);
            await Console.Out.WriteLineAsync();
        }
        [Fact]
        public async Task Test6()
        {
            var conventionPack = new ConventionPack
            {
                new MapReadOnlyPropertiesConvention()
            };
            ConventionRegistry.Register("Conventions", conventionPack, _ => true);
           
            BsonClassMap.RegisterClassMap<RoomAccessControl>(classMap =>
            {
                classMap.AutoMap();
                classMap.MapField("_users").SetElementName("Users");
                classMap.MapField("_roomSpecification").SetElementName("RoomSpecification");
                classMap.UnmapProperty(c => c.Users);


            });
            var mediatr = new Mock<IMediator>();
            var changeTracker = new InMemoryChangeTracker();
            var mongoContext = new MongoContext(new MongoDBOptions { Connection = "mongodb://localhost:27017", DatabaseName = "MyTestDb" }, changeTracker, mediatr.Object);
            var rep = new RoomAccessControlRepository(mongoContext, changeTracker);

            var id = Guid.NewGuid();
            var user = new User(Guid.NewGuid(), RoleType.Admin);
            var room = RoomAccessControl.Create(id, user);

            rep.Add(room);

            await rep.UnitOfWork.SaveChangesAsync();
            var r = await rep.GetRoom(id);
            await Console.Out.WriteLineAsync();
        }
    }
}