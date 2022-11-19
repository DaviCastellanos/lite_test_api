using Microsoft.Azure.Cosmos;

namespace lite_test.Infrastructure.CosmosDbData.Interfaces
{
    public interface ICosmosDbContainer
    {
        Container _container { get; }
    }
}
