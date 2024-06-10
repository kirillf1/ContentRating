using ContentRating.Domain.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContentRatingAPI.Infrastructure.Data
{
    public class MongoContext : IUnitOfWork
    {

        public MongoContext(IOptions<MongoDBOptions> options, IChangeTracker changeTracker, IMediator mediator, MongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase(options.Value.DatabaseName);
            _changeTracker = changeTracker;
            _mediator = mediator;
            _commands = new List<Func<IClientSessionHandle, Task>>();
            _mongoClient = mongoClient;
        }
        private IMongoDatabase _database;
        private IClientSessionHandle? _scopedSession;
        private MongoClient _mongoClient;
        private readonly List<Func<IClientSessionHandle, Task>> _commands;
        private readonly IChangeTracker _changeTracker;
        private readonly IMediator _mediator;
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _scopedSession ??= await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
            _scopedSession.StartTransaction();
        }
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_scopedSession is null)
                throw new ArgumentNullException(nameof(_scopedSession));

            await _scopedSession.CommitTransactionAsync(cancellationToken);
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_scopedSession is null)
            {
                using var session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
                session.StartTransaction();
                var commandsCount = await ExecuteCommands(session);
                session.CommitTransaction(cancellationToken);
                return commandsCount;
            }
            return await ExecuteCommands(_scopedSession);
        }

        private async Task<int> ExecuteCommands(IClientSessionHandle currentSession)
        {
            await ExecuteDomainEvents();

            var commandTasks = _commands.Select(c => c(currentSession));

            foreach (var commandTask in commandTasks)
            {
                await commandTask;
            }
            var commandsCount = _commands.Count;
            _commands.Clear();
            return commandsCount;
        }

        public IMongoCollection<T> GetCollection<T>(string name) 
        { 
            return _database.GetCollection<T>(name);
        }

        public void Dispose()
        {
            _scopedSession?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void AddCommand(Func<IClientSessionHandle, Task> func, Entity? targetEntity)
        {
            _commands.Add(func);
            if (targetEntity is not null)
                _changeTracker.TrackEntity(targetEntity);
        }

        private async Task ExecuteDomainEvents()
        {
            var domainEntities = _changeTracker.GetTrackedEntities().Where(c => c.DomainEvents?.Count > 0);

            var domainEvents = domainEntities.SelectMany(x => x.DomainEvents).ToList();
            domainEntities.ToList().ForEach(entity => entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent);
        }

    }
}

