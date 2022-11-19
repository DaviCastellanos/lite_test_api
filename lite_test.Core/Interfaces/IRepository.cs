using lite_test.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lite_test.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetItemsAsync(string query);

        Task<T> GetItemAsync(string id);

        Task AddItemAsync(T item);

        Task UpdateItemAsync(T item);

        Task DeleteItemAsync(string id);

    }
}
