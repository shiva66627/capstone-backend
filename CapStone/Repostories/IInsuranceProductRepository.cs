using CapStone.Models;

namespace CapStone.Repositories
{
    public interface IInsuranceProductRepository : IGenericRepository<InsuranceProduct>
    {
        Task<InsuranceProduct?> GetByProductNameAsync(string productName);

        Task<bool> ProductExistsAsync(string productName);

        Task<IEnumerable<InsuranceProduct>> GetAvailableProductsAsync();
    }
}