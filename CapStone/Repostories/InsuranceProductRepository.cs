using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;

namespace CapStone.Repositories
{
    public class InsuranceProductRepository : GenericRepository<InsuranceProduct>, IInsuranceProductRepository
    {
        public InsuranceProductRepository(CapstoneDbContext context)
            : base(context)
        {
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
    }
}