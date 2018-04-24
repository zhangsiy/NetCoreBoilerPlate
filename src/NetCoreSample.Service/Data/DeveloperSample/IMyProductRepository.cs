using NetCoreSample.Service.Models.DeveloperSample;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreSample.Service.Data.DeveloperSample
{
    public interface IMyProductRepository
    {
        Task<IEnumerable<MyProduct>> GetAllAsync();

        Task<MyProduct> GetAsync(string id);

        Task<MyProduct> UpdateAsync(MyProduct objectToUpdate);

        Task<MyProduct> CreateAsync(MyProduct objectToCreate);
    }
}
