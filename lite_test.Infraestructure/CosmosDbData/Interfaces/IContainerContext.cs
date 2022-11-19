using lite_test.Core.Entities;
using Microsoft.Azure.Cosmos;

namespace lite_test.Infrastructure.CosmosDbData.Interfaces
{
    public interface IContainerContext<T> where T : BaseEntity
    {
        string ContainerName { get; }
        string GenerateId(T entity);
    }
}
