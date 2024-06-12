using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.Domain.Shared.Content;
using ContentRatingAPI.Application.ContentLoader;
using ContentRatingAPI.Infrastructure.AggregateIntegration.ContentPartyRating;
using ContentRatingAPI.Infrastructure.Data;
using ContentRatingAPI.Infrastructure.Data.MapConvensions;
using ContentRatingAPI.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;


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

            using (var Session = await mongoClient.StartSessionAsync())
            {
                Session.StartTransaction();
                try
                {
                    var c1 = d.GetCollection<TestEntity1>("rooms1");
                    var e = new TestEntity(Guid.NewGuid(), "test", "t");
                    
                    var c = d.GetCollection<TestEntity>("rooms");
                    c.InsertOne(Session, e);
                    c1.InsertOne(Session, new TestEntity1(Guid.NewGuid(), "tet", "test"));
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
            var options = Options.Create( new MongoDBOptions { Connection = "mongodb://localhost:27017", DatabaseName = "MyTestDb" });
            var client = new MongoClient(options.Value.Connection);
            var mongoContext = new MongoContext(options, changeTracker, mediatr.Object, client);
            var rep = new ContentEditorRoomRepository(mongoContext, options);
            await mongoContext.BeginTransactionAsync();
            var id = Guid.NewGuid();
            var editor = new Editor(Guid.NewGuid(), "test");
            var newRoom = new ContentRoomEditor(id, editor, "testRoom");
            newRoom.CreateContent(editor, new ContentData(Guid.NewGuid(), "test", "test", ContentType.Image));
            rep.Add(newRoom);

            await rep.UnitOfWork.SaveChangesAsync();
            await mongoContext.CommitAsync();
            var room = await rep.GetRoom(id);
            var r = await rep.HasEditorInRoom(id, Guid.NewGuid());
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

                BsonClassMap.RegisterClassMap<ContentPartyRating>(classMap =>
            {
                classMap.AutoMap();;
                classMap.MapField(c => c.Id).SetElementName("ContentId");
                //classMap.SetDictionaryRepresentation(memberName: "_raterScores", representation: DictionaryRepresentation.ArrayOfDocuments);
                classMap.SetDictionaryRepresentation(c => c.RaterScores, DictionaryRepresentation.ArrayOfDocuments);

            });
            var mediatr = new Mock<IMediator>();
            var changeTracker = new InMemoryChangeTracker();
            var options = Options.Create(new MongoDBOptions
            {
                Connection = "mongodb://localhost:27017",
                DatabaseName = "MyTestDb",
                ContentPartyEstimationRoomCollectionName = "PartyEstimation",
                ContentPartyRatingCollectionName = "ContentPartyRating"
            });
            var client = new MongoClient(options.Value.Connection);
            var mongoContext = new MongoContext(options, changeTracker, mediatr.Object, client);
          

            // Создание индексов, стоит засунуть в Onstartap и добавить в context функционал добавлениея индексов

            var indexKeysDefinition = Builders<ContentPartyRating>.IndexKeys.Ascending(c => c.Id).Ascending(c => c.RoomId);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<ContentPartyRating>(indexKeysDefinition, indexOptions);
            var c = mongoContext.GetCollection<ContentPartyRating>(options.Value.ContentPartyRatingCollectionName);
            c.Indexes.CreateOne(indexModel);
  
            c.Indexes.CreateOne(indexModel);
            await mongoContext.BeginTransactionAsync();
            var rep = new ContentPartyRatingRepository(mongoContext, options);

            var id = Guid.NewGuid();
            var spec = new ContentRatingSpecification(new Score(0), new Score(10));
            var raters = new List<ContentRater> { new ContentRater(Guid.NewGuid(), RaterType.Admin), new ContentRater(Guid.NewGuid(), RaterType.Admin) };
            var rating = ContentPartyRating.Create(id, Guid.NewGuid(), spec);
            var s = rating.AverageContentScore;
            foreach (var rater in raters)
            {
                rating.AddNewRaterInContentEstimation(rater);
            }
           
            rep.Add(rating);
            rating = ContentPartyRating.Create(id, Guid.NewGuid(), spec);
            rep.Add(rating);
            await rep.UnitOfWork.SaveChangesAsync();
            await mongoContext.CommitAsync();
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
           
            BsonClassMap.RegisterClassMap<ContentPartyEstimationRoom>(classMap =>
            {
                classMap.AutoMap();
                classMap.MapProperty(c => c.Raters);


            });
            BsonClassMap.RegisterClassMap<ContentPartyRating>(classMap =>
            {
                classMap.AutoMap(); ;
                //classMap.SetDictionaryRepresentation(memberName: "_raterScores", representation: DictionaryRepresentation.ArrayOfDocuments);
                classMap.SetDictionaryRepresentation(c => c.RaterScores, DictionaryRepresentation.ArrayOfDocuments);

            });
            var mediatr = new Mock<IMediator>();
            var changeTracker = new InMemoryChangeTracker();
            var options = Options.Create(new MongoDBOptions { Connection = "mongodb://localhost:27017", DatabaseName = "MyTestDb", 
                ContentPartyEstimationRoomCollectionName = "PartyEstimation", ContentPartyRatingCollectionName = "ContentPartyRating" });
            var client = new MongoClient(options.Value.Connection);
            var mongoContext = new MongoContext(options, changeTracker, mediatr.Object, client);
            var rep = new ContentPartyEstimationRoomRepository(mongoContext, options);;
            var id = Guid.NewGuid();
            var user = new Rater(Guid.NewGuid(), RoleType.Admin, "test");
            var room = ContentPartyEstimationRoom.Create(id, user, [new ContentForEstimation(Guid.NewGuid(),"testName","/test/url", ContentType.Audio)]);
            room.InviteRater(new Rater(Guid.NewGuid(), RoleType.Admin, "test"), user.Id);

            rep.Add(room);

            await rep.UnitOfWork.SaveChangesAsync();
            var adapter = new ContentRaterAdapter(new ContentRaterTranslator(), mongoContext, options);
            var a = await adapter.GetContentRates(id, room.Raters.Select(c => c.Id).ToArray());
            var rep1 = new ContentPartyRatingRepository(mongoContext, options);

            var id1 = Guid.NewGuid();
            var spec = new ContentRatingSpecification(new Score(0), new Score(10));
            var raters = new List<ContentRater> { new ContentRater(Guid.NewGuid(), RaterType.Admin), new ContentRater(Guid.NewGuid(), RaterType.Admin) };
            var rating = ContentPartyRating.Create(id1, Guid.NewGuid(), spec);
            foreach (var rater in raters)
            {
                rating.AddNewRaterInContentEstimation(rater);
            }
            rep1.Add(rating);

            await rep1.UnitOfWork.SaveChangesAsync();
            
            var r1 = await rep1.GetContentRating(id1);
            var r = await rep.GetRoom(id);
            var res = await rep.HasRaterInRoom(id, user.Id);
            await Console.Out.WriteLineAsync();
        }
        [Fact]
        public async Task Test7()
        {
            var loader = new ContentLoaderService();
            await loader.SaveToHLS(@"C:\Users\kirill\Downloads\Limp Bizkit - My Way.mp4", @"D:\Videos", "limp");
        }


    }

    }
