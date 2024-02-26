using ContentRating.Domain.AggregatesModel.ContentRoomAggregate;
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
            var c = d.GetCollection<TestEntity>("rooms");
            var e = c.Find(Builders<TestEntity>.Filter.Eq(c => c.Id, new Guid("b1cec36a-97d4-4b89-8baf-c18612f2454d"))).First();
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

                    var e = new TestEntity(Guid.NewGuid(), "test", "t");
                    c.InsertOne(Session ,e);
                    using (var session = await mongoClient.StartSessionAsync())
                    {
                        session.StartTransaction();
                        e = new TestEntity(Guid.NewGuid(), "test", "t");
                        c.InsertOne(session, e);
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

            var newRoom = new Room(Guid.NewGuid(), new User(Guid.NewGuid(), "test", false), "testRoom");
            rep.Add(newRoom);

            await rep.UnitOfWork.SaveChangesAsync();
        }
    }
}