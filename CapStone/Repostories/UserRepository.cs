using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;

namespace CapStone.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(CapstoneDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByMobileNumberAsync(string mobileNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> MobileExistsAsync(string mobileNumber)
        {
            return await _context.Users
                .AnyAsync(u => u.MobileNumber == mobileNumber);
        }
    }
}