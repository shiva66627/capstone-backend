using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CapstoneDbContext _context;

        public CustomerRepository(CapstoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.User)
                .Where(c => c.User != null && c.User.Role == Role.Customer)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task<IEnumerable<Customer>> FindAsync(Expression<Func<Customer, bool>> predicate)
        {
            return await _context.Customers.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(Customer entity)
        {
            await _context.Customers.AddAsync(entity);
        }

        public Task UpdateAsync(Customer entity)
        {
            _context.Customers.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Customers.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
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

        public async Task<(IEnumerable<Customer> Data, int TotalCount)> GetFilteredCustomersAsync(CapStone.DTO.Customer.CustomerFilterDTO filter)
        {
            var query = _context.Customers
                .Include(c => c.User)
                .Where(c => c.User != null && c.User.Role == Role.Customer)
                .AsQueryable();

            // Sorting
            if (filter.SortDirection.ToLower() == "asc")
            {
                query = filter.SortField.ToLower() switch
                {
                    "city" => query.OrderBy(c => c.City),
                    "state" => query.OrderBy(c => c.State),
                    _ => query.OrderBy(c => c.CreatedDate)
                };
            }
            else
            {
                query = filter.SortField.ToLower() switch
                {
                    "city" => query.OrderByDescending(c => c.City),
                    "state" => query.OrderByDescending(c => c.State),
                    _ => query.OrderByDescending(c => c.CreatedDate)
                };
            }

            return await query.GetPagedDataAsync(filter.PageNumber, filter.PageSize);
        }
    }
}