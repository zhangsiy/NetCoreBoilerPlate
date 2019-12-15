using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NetCoreSample.Configurations.DeveloperSample;
using NetCoreSample.Data.WebServices;
using NetCoreSample.Models.DeveloperSample;

namespace NetCoreSample.Data.DeveloperSample
{
    internal class MyProductRepository : WebServiceRepositoryBase, IMyProductRepository
    {
        private static List<MyProduct> AllMyProducts = new List<MyProduct>
        {
            new MyProduct
                {
                    MyProductId = "AAA-001",
                    Name = "My Product 1",
                    Description = "First test MyProduct",
                    Created = new DateTime(2015, 10, 22, 10, 10, 10),
                    Modified = new DateTime(2015, 10, 22, 10, 10, 10)
                },
                new MyProduct
                {
                    MyProductId = "AAA-002",
                    Name = "My Product 2",
                    Description = "Second test MyProduct",
                    Created = new DateTime(2016, 2, 3, 4, 30, 30),
                    Modified = new DateTime(2016, 3, 12, 7, 8, 9)
                },
                new MyProduct
                {
                    MyProductId = "AAA-003",
                    Name = "My Product 3",
                    Description = "Third test MyProduct",
                    Created = new DateTime(2015, 03, 15, 15, 22, 11),
                    Modified = new DateTime(2016, 05, 27, 19, 6, 3)
                }
        };

        private ServiceDependenciesConfig ServiceDependenciesConfig { get; set; }

        public MyProductRepository(HttpClient httpClient, IOptions<ServiceDependenciesConfig> serviceDependenciesConfig)
            : base(httpClient)
        {
            ServiceDependenciesConfig = serviceDependenciesConfig.Value;
        }

        public async Task<MyProduct> CreateAsync(MyProduct objectToCreate)
        {
            if (!string.IsNullOrEmpty(objectToCreate.MyProductId))
            {
                throw new ArgumentException($"The given '{nameof(objectToCreate)}' already has an ID!");
            }

            // "Create" the new Entity
            // Generate a GUID to serve as the object ID
            objectToCreate.MyProductId = Guid.NewGuid().ToString();
            objectToCreate.Created = objectToCreate.Modified = DateTime.UtcNow;
            AllMyProducts.Add(objectToCreate);

            return await Task.FromResult(objectToCreate);
        }

        public async Task<IEnumerable<MyProduct>> GetAllAsync()
        {
            return await Task.FromResult(AllMyProducts);
        }

        public async Task<MyProduct> GetAsync(string id)
        {
            return await Task.FromResult(AllMyProducts.FirstOrDefault(p => p.MyProductId == id));
        }

        public async Task<MyProduct> UpdateAsync(MyProduct objectToUpdate)
        {
            if (string.IsNullOrEmpty(objectToUpdate.MyProductId))
            {
                throw new ArgumentException($"The given '{nameof(objectToUpdate)}' does not have an ID!");
            }

            objectToUpdate.Modified = DateTime.UtcNow;

            // Simulate an update by removing target and add the new value
            MyProduct target = AllMyProducts.First(p => p.MyProductId == objectToUpdate.MyProductId);
            AllMyProducts.Remove(target);
            AllMyProducts.Add(objectToUpdate);

            return await Task.FromResult(objectToUpdate);
        }
    }
}
