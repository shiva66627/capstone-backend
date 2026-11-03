using CapStone.Models;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public interface IInsuranceProductRepository
    {
        Task<IEnumerable<InsuranceProduct>> GetAllAsync();
        Task<InsuranceProduct?> GetByIdAsync(int id);
        Task<IEnumerable<InsuranceProduct>> FindAsync(Expression<Func<InsuranceProduct, bool>> predicate);
        Task AddAsync(InsuranceProduct entity);
        Task UpdateAsync(InsuranceProduct entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

        Task<InsuranceProduct?> GetByProductNameAsync(string productName);

        Task<bool> ProductExistsAsync(string productName);

        Task<IEnumerable<InsuranceProduct>> GetAvailableProductsAsync();

        Task<(IEnumerable<InsuranceProduct> Data, int TotalCount)> GetFilteredProductsAsync(CapStone.DTO.InsuranceProduct.ProductFilterDTO filter);
    }
}