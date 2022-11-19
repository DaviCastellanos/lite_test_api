using lite_test.Core.Entities;
using lite_test.Core.Interfaces;
using lite_test.Infrastructure.CosmosDbData.Interfaces;
using lite_test.Infrastructure.CosmosDbData.Repository;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lite_test.Infrastructure.CosmosDbData.Repository
{
    public class BusinessRepository : CosmosDbRepository<BusinessItem>, IBusinessRepository
    {
        /// <summary>
        ///     CosmosDB container name
        /// </summary>
        public override string ContainerName { get; } = "business";

        /// <summary>
        ///     Generate Id.
        ///     e.g. "shoppinglist:783dfe25-7ece-4f0b-885e-c0ea72135942"
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override string GenerateId(BusinessItem entity) => $"{entity.Id}:{Guid.NewGuid()}";

        /// <summary>
        ///     Returns the value of the partition key
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId.Split(':')[0]);

        public BusinessRepository(ICosmosDbContainerFactory factory) : base(factory)
        { }

        // Use Cosmos DB Parameterized Query to avoid SQL Injection.
        // Get by Category is also an example of single partition read, where get by title will be a cross partition read
        public async Task<IEnumerable<BusinessItem>> GetItemsAsyncByCategory(string category)
        {
            List<BusinessItem> results = new List<BusinessItem>();
            string query = @$"SELECT c.Name FROM c WHERE c.Category = @Category";

            QueryDefinition queryDefinition = new QueryDefinition(query)
                                                    .WithParameter("@Category", category);
            string queryString = queryDefinition.QueryText;

            IEnumerable<BusinessItem> entities = await this.GetItemsAsync(queryString);

            return results;
        }

        // Use Cosmos DB Parameterized Query to avoid SQL Injection.
        // Get by Title is also an example of cross partition read, where Get by Category will be single partition read
        public async Task<IEnumerable<BusinessItem>> GetBusinessAsyncByNIT(string NIT)
        {
            string query = $"SELECT * FROM c WHERE c.NIT = '{NIT}'";

            IEnumerable<BusinessItem> entities = await this.GetItemsAsync(query);

            return entities;
        }
    }
}
