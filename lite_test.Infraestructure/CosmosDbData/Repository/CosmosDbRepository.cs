using Ardalis.Specification;
using lite_test.Core.Entities;
using lite_test.Core.Interfaces;
using lite_test.Infrastructure.CosmosDbData.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace lite_test.Infrastructure.CosmosDbData.Repository
{
    public abstract class CosmosDbRepository<T> : IRepository<T>, IContainerContext<T> where T : BaseEntity
    {
        public abstract string ContainerName { get; }

        public abstract string GenerateId(T entity);

        public virtual PartitionKey ResolveAuditPartitionKey(string entityId) => new PartitionKey($"{entityId.Split(':')[0]}:{entityId.Split(':')[1]}");

        private readonly ICosmosDbContainerFactory _cosmosDbContainerFactory;

        private readonly Microsoft.Azure.Cosmos.Container _container;

        private readonly Microsoft.Azure.Cosmos.Container _auditContainer;

        public CosmosDbRepository(ICosmosDbContainerFactory cosmosDbContainerFactory)
        {
            this._cosmosDbContainerFactory = cosmosDbContainerFactory ?? throw new ArgumentNullException(nameof(ICosmosDbContainerFactory));
            this._container = this._cosmosDbContainerFactory.GetContainer(ContainerName)._container;
            this._auditContainer = this._cosmosDbContainerFactory.GetContainer("Audit")._container;
        }

        public async Task AddItemAsync(T item)
        {
            item.Id = GenerateId(item);
            await _container.CreateItemAsync<T>(item, new PartitionKey(item.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(string queryString)
        {
            FeedIterator<T> resultSetIterator = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            List<T> results = new List<T>();
            while (resultSetIterator.HasMoreResults)
            {
                FeedResponse<T> response = await resultSetIterator.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetItemsAsyncLinq()
        {
            List<T> results = new List<T>();

            using (FeedIterator<T> setIterator = _container.GetItemLinqQueryable<T>().ToFeedIterator())
            {
                while (setIterator.HasMoreResults)
                {
                    FeedResponse<T> response = await setIterator.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
            }

            return results;
        }

        public async Task UpdateItemAsync(T item)
        {
            await this._container.UpsertItemAsync<T>(item, new PartitionKey(item.Id));
        }

    }
}
