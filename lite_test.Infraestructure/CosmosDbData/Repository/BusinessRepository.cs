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
        public override string ContainerName { get; } = "business";

        public override string GenerateId(BusinessItem entity) => $"{entity.Id}:{Guid.NewGuid()}";

        public BusinessRepository(ICosmosDbContainerFactory factory) : base(factory)
        { }


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

        public async Task<IEnumerable<BusinessItem>> GetBusinessAsyncByNIT(string NIT)
        {
            string query = $"SELECT * FROM c WHERE c.NIT = '{NIT}'";

            IEnumerable<BusinessItem> entities = await this.GetItemsAsync(query);

            return entities;
        }

        public async Task<IEnumerable<BusinessItem>> GetAllBusiness()
        {

            IEnumerable<BusinessItem> entities = await this.GetItemsAsyncLinq();

            return entities;
        }
    }
}
