using CapStone.Models;

namespace CapStone.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByMobileNumberAsync(string mobileNumber);

        Task<bool> EmailExistsAsync(string email);

        Task<bool> MobileExistsAsync(string mobileNumber);
    }
}