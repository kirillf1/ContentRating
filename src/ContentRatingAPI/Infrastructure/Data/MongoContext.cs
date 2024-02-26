using ContentRating.Domain.Shared;
using MediatR;
using MongoDB.Driver;

namespace ContentRatingAPI.Infrastructure.Data
{
    public class MongoContext : IUnitOfWork
    {

        public MongoContext(MongoDBOptions options, IChangeTracker changeTracker, IMediator mediator)
        {
            _options = options;
            _changeTracker = changeTracker;
            _mediator = mediator;
            _commands = new List<Func<IClientSessionHandle, Task>>();
        }
        private IMongoDatabase Database { get; set; }
        public IClientSessionHandle Session { get; set; }
        public MongoClient MongoClient { get; set; }
        private readonly List<Func<IClientSessionHandle, Task>> _commands;
        private readonly MongoDBOptions _options;
        private readonly IChangeTracker _changeTracker;
        private readonly IMediator _mediator;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConfigureMongo();

            using (Session = await MongoClient.StartSessionAsync(cancellationToken: cancellationToken))
            {
                try
                {
                    Session.StartTransaction();

                    await ExecuteDomainEvents();

                    var commandTasks = _commands.Select(c => c(Session));

                    await Task.WhenAll(commandTasks);

                    await Session.CommitTransactionAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    await Session.AbortTransactionAsync(cancellationToken);
                    throw;
                }
            }

            return _commands.Count;
        }


        public IMongoCollection<T> GetCollection<T>(string name)
        {
            ConfigureMongo();

            return Database.GetCollection<T>(name);
        }

        public void Dispose()
        {
            Session?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void AddCommand(Func<IClientSessionHandle, Task> func)
        {
            _commands.Add(func);
        }

        private async Task ExecuteDomainEvents()
        {
            var domainEntities = _changeTracker.GetTrackedEntities().Where(c => c.DomainEvents?.Count > 0);

            var domainEvents = domainEntities.SelectMany(x => x.DomainEvents).ToList();
            domainEntities.ToList().ForEach(entity => entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent);
        }

        private void ConfigureMongo()
        {
            if (MongoClient != null)
            {
                return;
            }
            // TODO не рекомендуется создавать клиент на каждый запрос
            MongoClient = new MongoClient(_options.Connection);

            Database = MongoClient.GetDatabase(_options.DatabaseName);
        }
    }
}

