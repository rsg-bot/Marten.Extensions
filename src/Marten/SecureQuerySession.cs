﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Linq;
using Marten.Services.BatchQuerying;
using Marten.Storage;
using Npgsql;
using Rocket.Surgery.Extensions.Marten.Security;

namespace Rocket.Surgery.Extensions.Marten
{
    public interface ISecureQuerySession : IQuerySession {}
    public class SecureQuerySession : ISecureQuerySession
    {
        private readonly IQuerySession _querySession;
        private readonly ISecurityQueryProvider _securityQueryProvider;
        private readonly IMartenContext _martenContext;

        public SecureQuerySession(IQuerySession querySession, ISecurityQueryProvider securityQueryProvider, IMartenContext martenContext)
        {
            _querySession = querySession;
            _securityQueryProvider = securityQueryProvider;
            _martenContext = martenContext;
        }

        public void Dispose()
        {
            _querySession.Dispose();
        }

        public T Load<T>(string id)
        {
            return _querySession.Load<T>(id);
        }

        public Task<T> LoadAsync<T>(string id, CancellationToken token = new CancellationToken())
        {
            return _querySession.LoadAsync<T>(id, token);
        }

        public T Load<T>(int id)
        {
            return _querySession.Load<T>(id);
        }

        public T Load<T>(long id)
        {
            return _querySession.Load<T>(id);
        }

        public T Load<T>(Guid id)
        {
            return _querySession.Load<T>(id);
        }

        public Task<T> LoadAsync<T>(int id, CancellationToken token = new CancellationToken())
        {
            return _querySession.LoadAsync<T>(id, token);
        }

        public Task<T> LoadAsync<T>(long id, CancellationToken token = new CancellationToken())
        {
            return _querySession.LoadAsync<T>(id, token);
        }

        public Task<T> LoadAsync<T>(Guid id, CancellationToken token = new CancellationToken())
        {
            return _querySession.LoadAsync<T>(id, token);
        }

        public IMartenQueryable<T> Query<T>()
        {
            return (IMartenQueryable<T>) _querySession.Query<T>()
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext);
        }

        public IReadOnlyList<T> Query<T>(string sql, params object[] parameters)
        {
            return _querySession.Query<T>(sql, parameters)
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, CancellationToken token = new CancellationToken(), params object[] parameters)
        {
            return (await _querySession.QueryAsync<T>(sql, token, parameters).ConfigureAwait(false))
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public IBatchedQuery CreateBatchQuery()
        {
            return _querySession.CreateBatchQuery();
        }

        public TOut Query<TDoc, TOut>(ICompiledQuery<TDoc, TOut> query)
        {
            return _querySession.Query(query);
        }

        public Task<TOut> QueryAsync<TDoc, TOut>(ICompiledQuery<TDoc, TOut> query, CancellationToken token = new CancellationToken())
        {
            return _querySession.QueryAsync(query, token);
        }

        public IReadOnlyList<T> LoadMany<T>(params string[] ids)
        {
            return _querySession.LoadMany<T>(ids)
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public IReadOnlyList<T> LoadMany<T>(params Guid[] ids)
        {
            return _querySession.LoadMany<T>(ids)
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public IReadOnlyList<T> LoadMany<T>(params int[] ids)
        {
            return _querySession.LoadMany<T>(ids)
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public IReadOnlyList<T> LoadMany<T>(params long[] ids)
        {
            return _querySession.LoadMany<T>(ids)
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public async Task<IReadOnlyList<T>> LoadManyAsync<T>(params string[] ids)
        {
            return (await _querySession.LoadManyAsync<T>(ids).ConfigureAwait(false))
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public async Task<IReadOnlyList<T>> LoadManyAsync<T>(params Guid[] ids)
        {
            return (await _querySession.LoadManyAsync<T>(ids).ConfigureAwait(false))
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public async Task<IReadOnlyList<T>> LoadManyAsync<T>(params int[] ids)
        {
            return (await _querySession.LoadManyAsync<T>(ids).ConfigureAwait(false))
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public async Task<IReadOnlyList<T>> LoadManyAsync<T>(params long[] ids)
        {
            return (await _querySession.LoadManyAsync<T>(ids).ConfigureAwait(false))
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public async Task<IReadOnlyList<T>> LoadManyAsync<T>(CancellationToken token, params string[] ids)
        {
            return (await _querySession.LoadManyAsync<T>(token, ids).ConfigureAwait(false))
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public async Task<IReadOnlyList<T>> LoadManyAsync<T>(CancellationToken token, params Guid[] ids)
        {
            return (await _querySession.LoadManyAsync<T>(token, ids).ConfigureAwait(false))
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public async Task<IReadOnlyList<T>> LoadManyAsync<T>(CancellationToken token, params int[] ids)
        {
            return (await _querySession.LoadManyAsync<T>(token, ids).ConfigureAwait(false))
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public async Task<IReadOnlyList<T>> LoadManyAsync<T>(CancellationToken token, params long[] ids)
        {
            return (await _querySession.LoadManyAsync<T>(token, ids).ConfigureAwait(false))
                .OnlyItemsTheUserCanSee(_securityQueryProvider, _martenContext)
                .ToList();
        }

        public Guid? VersionFor<TDoc>(TDoc entity)
        {
            return _querySession.VersionFor(entity);
        }

        public IReadOnlyList<TDoc> Search<TDoc>(string queryText, string config = "english")
        {
            return _querySession.Search<TDoc>(queryText, config);
        }

        public Task<IReadOnlyList<TDoc>> SearchAsync<TDoc>(string queryText, string config = "english", CancellationToken token = default)
        {
            return _querySession.SearchAsync<TDoc>(queryText, config);
        }

        public IReadOnlyList<TDoc> PlainTextSearch<TDoc>(string searchTerm, string config = "english")
        {
            return _querySession.PlainTextSearch<TDoc>(searchTerm, config);
        }

        public Task<IReadOnlyList<TDoc>> PlainTextSearchAsync<TDoc>(string searchTerm, string config = "english", CancellationToken token = default)
        {
            return _querySession.PlainTextSearchAsync<TDoc>(searchTerm, config);
        }

        public IReadOnlyList<TDoc> PhraseSearch<TDoc>(string searchTerm, string config = "english")
        {
            return _querySession.PhraseSearch<TDoc>(searchTerm, config);
        }

        public Task<IReadOnlyList<TDoc>> PhraseSearchAsync<TDoc>(string searchTerm, string config = "english", CancellationToken token = default)
        {
            return _querySession.PhraseSearchAsync<TDoc>(searchTerm, config);
        }

        public IReadOnlyList<TDoc> WebStyleSearch<TDoc>(string searchTerm, string regConfig = "english")
        {
            return _querySession.WebStyleSearch<TDoc>(searchTerm, regConfig);
        }

        public Task<IReadOnlyList<TDoc>> WebStyleSearchAsync<TDoc>(string searchTerm, string regConfig = "english",
            CancellationToken token = new CancellationToken())
        {
            return _querySession.WebStyleSearchAsync<TDoc>(searchTerm, regConfig);
        }

        public NpgsqlConnection Connection => _querySession.Connection;

        public IMartenSessionLogger Logger
        {
            get => _querySession.Logger;
            set => _querySession.Logger = value;
        }

        public int RequestCount => _querySession.RequestCount;

        public IDocumentStore DocumentStore => _querySession.DocumentStore;

        public IJsonLoader Json => _querySession.Json;

        public ITenant Tenant => _querySession.Tenant;

        public ISerializer Serializer => _querySession.Serializer;
    }
}
