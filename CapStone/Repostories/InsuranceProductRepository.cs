using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public class InsuranceProductRepository : IInsuranceProductRepository
    {
        private readonly CapstoneDbContext _context;

        public InsuranceProductRepository(CapstoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InsuranceProduct>> GetAllAsync()
        {
            return await _context.InsuranceProducts.ToListAsync();
        }

        public async Task<InsuranceProduct?> GetByIdAsync(int id)
        {
            return await _context.InsuranceProducts.FindAsync(id);
        }

        public async Task<IEnumerable<InsuranceProduct>> FindAsync(Expression<Func<InsuranceProduct, bool>> predicate)
        {
            return await _context.InsuranceProducts.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(InsuranceProduct entity)
        {
            await _context.InsuranceProducts.AddAsync(entity);
        }

        public Task UpdateAsync(InsuranceProduct entity)
        {
            _context.InsuranceProducts.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.InsuranceProducts.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<InsuranceProduct?> GetByProductNameAsync(string productName)
        {
            return await _context.InsuranceProducts
                .FirstOrDefaultAsync(p => p.ProductName == productName);
        }

        public async Task<bool> ProductExistsAsync(string productName)
        {
            return await _context.InsuranceProducts
                .AnyAsync(p => p.ProductName == productName);
        }

        public async Task<IEnumerable<InsuranceProduct>> GetAvailableProductsAsync()
        {
            return await _context.InsuranceProducts
                .Where(p => p.Status == ActiveStatus.Available)
                .ToListAsync();
        }

        public async Task<(IEnumerable<InsuranceProduct> Data, int TotalCount)> GetFilteredProductsAsync(CapStone.DTO.InsuranceProduct.ProductFilterDTO filter)
        {
            var query = _context.InsuranceProducts.AsQueryable();

            if (!string.IsNullOrEmpty(filter.ProductType))
            {
                if (Enum.TryParse<ProductType>(filter.ProductType, true, out var parsedType))
                    query = query.Where(p => p.ProductType == parsedType);
            }

            if (filter.IsActive.HasValue)
            {
                var status = filter.IsActive.Value ? ActiveStatus.Available : ActiveStatus.NotAvailable;
                query = query.Where(p => p.Status == status);
            }

            if (filter.SortDirection.ToLower() == "asc")
            {
                query = filter.SortField.ToLower() switch
                {
                    "productname" => query.OrderBy(p => p.ProductName),
                    "producttype" => query.OrderBy(p => p.ProductType),
                    _ => query.OrderBy(p => p.CreatedDate)
                };
            }
            else
            {
                query = filter.SortField.ToLower() switch
                {
                    "productname" => query.OrderByDescending(p => p.ProductName),
                    "producttype" => query.OrderByDescending(p => p.ProductType),
                    _ => query.OrderByDescending(p => p.CreatedDate)
                };
            }

            return await query.GetPagedDataAsync(filter.PageNumber, filter.PageSize);
        }
    }
}