using CapStone.Models;

namespace CapStone.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByUserIdAsync(int userId);

        Task<bool> CustomerExistsAsync(int customerId);
    }
}