using System.Threading.Tasks;

namespace lite_test.Infrastructure.CosmosDbData.Interfaces
{
    public interface ICosmosDbContainerFactory
    {
        ICosmosDbContainer GetContainer(string containerName);
    }
}
