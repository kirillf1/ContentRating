using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.Domain.AggregatesModel.RoomEditorAggregate;
using ContentRatingAPI.Infrastructure.Data;
using ContentRatingAPI.Infrastructure.Data.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

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
                    c.InsertOne(Session ,e);
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
            var changeTracker = new InMemoryChangeTracker();
            var mongoContext = new MongoContext(new MongoDBOptions { Connection = "mongodb://localhost:27017", DatabaseName = "MyTestDb" }, changeTracker, null);
            var rep = new RoomRepository(mongoContext, changeTracker);

            var newRoom = new ContentRoomEditor(Guid.NewGuid(), new Editor(Guid.NewGuid(), "test"), "testRoom");
            rep.Add(newRoom);

            await rep.UnitOfWork.SaveChangesAsync();
        }
    }
}