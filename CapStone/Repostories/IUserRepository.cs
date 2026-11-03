using CapStone.Models;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate);
        Task AddAsync(User entity);
        Task UpdateAsync(User entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByMobileNumberAsync(string mobileNumber);

        Task<bool> EmailExistsAsync(string email);

        Task<bool> MobileExistsAsync(string mobileNumber);

        Task<(IEnumerable<User> Data, int TotalCount)> GetFilteredUsersAsync(CapStone.DTO.User.UserFilterDTO filter);
    }
}