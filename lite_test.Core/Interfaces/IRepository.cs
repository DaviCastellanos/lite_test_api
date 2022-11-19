using lite_test.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lite_test.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        ///     Get items given a string SQL query directly.
        ///     Likely in production, you may want to use alternatives like Parameterized Query or LINQ to avoid SQL Injection and avoid having to work with strings directly.
        ///     This is kept here for demonstration purpose.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetItemsAsync(string query);

        /// <summary>
        ///     Get one item by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetItemAsync(string id);

        /// <summary>
        ///     Add one item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task AddItemAsync(T item);

        /// <summary>
        ///     Updates an item
        /// </summary>
        /// <param name="id, item"></param>
        /// <returns></returns>
        Task UpdateItemAsync(T item);

        /// <summary>
        ///     Deletes an item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteItemAsync(string id);

    }
}
