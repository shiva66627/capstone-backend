using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;

namespace CapStone.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(CapstoneDbContext context)
            : base(context)
        {
        }

        public async Task<Customer?> GetByUserIdAsync(int userId)
        {
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            return await _context.Customers
                .AnyAsync(c => c.CustomerId == customerId);
        }
    }
}