using System.Collections.Generic;
using System.Threading.Tasks;
using NetCoreSample.Models.DeveloperSample;

namespace NetCoreSample.Data.DeveloperSample
{
    public interface IMyProductRepository
    {
        Task<IEnumerable<MyProduct>> GetAllAsync();

        Task<MyProduct> GetAsync(string id);

        Task<MyProduct> UpdateAsync(MyProduct objectToUpdate);

        Task<MyProduct> CreateAsync(MyProduct objectToCreate);
    }
}
