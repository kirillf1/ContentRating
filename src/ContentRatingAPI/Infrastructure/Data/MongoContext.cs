// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContentRatingAPI.Infrastructure.Data
{
    public class MongoContext : IUnitOfWork
    {
        public MongoContext(IOptions<MongoDBOptions> options, IChangeTracker changeTracker, IMediator mediator, IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase(options.Value.DatabaseName);
            _changeTracker = changeTracker;
            _mediator = mediator;
            _commands = new List<Func<IClientSessionHandle, Task>>();
            _mongoClient = mongoClient;
        }

        private readonly IMongoDatabase _database;
        private IClientSessionHandle? _scopedSession;
        private Guid? _transactionId;
        private readonly IMongoClient _mongoClient;
        private readonly List<Func<IClientSessionHandle, Task>> _commands;
        private readonly IChangeTracker _changeTracker;
        private readonly IMediator _mediator;

        public async Task<MongoTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _scopedSession ??= await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
            if (_scopedSession.IsInTransaction)
            {
                throw new MongoException("Transaction is started");
            }

            _scopedSession.StartTransaction();
            _transactionId = Guid.NewGuid();
            return new MongoTransaction(_transactionId.Value);
        }

        public bool HasActiveTransaction()
        {
            var isInTransaction = _scopedSession?.IsInTransaction;
            return isInTransaction.GetValueOrDefault();
        }

        public async Task CommitAsync(MongoTransaction transaction, CancellationToken cancellationToken = default)
        {
            if (_scopedSession is null || !_scopedSession.IsInTransaction)
            {
                throw new MongoException("Session not started");
            }

            if (transaction.TransactionId != _transactionId)
            {
                throw new MongoException("Unknown transaction");
            }

            await _scopedSession.CommitTransactionAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_scopedSession is null)
            {
                using var session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
                session.StartTransaction();
                var commandsCount = await ExecuteCommands(session);
                await session.CommitTransactionAsync(cancellationToken);
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
            {
                _changeTracker.TrackEntity(targetEntity);
            }
        }

        private async Task ExecuteDomainEvents()
        {
            var domainEntities = _changeTracker.GetTrackedEntities().Where(c => c.DomainEvents?.Count > 0);

            var domainEvents = domainEntities.SelectMany(x => x.DomainEvents).ToList();
            domainEntities.ToList().ForEach(entity => entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }
    }
}
