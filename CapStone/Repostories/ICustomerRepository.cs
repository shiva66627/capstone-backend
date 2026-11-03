using CapStone.Models;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task<IEnumerable<Customer>> FindAsync(Expression<Func<Customer, bool>> predicate);
        Task AddAsync(Customer entity);
        Task UpdateAsync(Customer entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

        Task<Customer?> GetByUserIdAsync(int userId);

        Task<bool> CustomerExistsAsync(int customerId);

        Task<(IEnumerable<Customer> Data, int TotalCount)> GetFilteredCustomersAsync(CapStone.DTO.Customer.CustomerFilterDTO filter);
    }
}