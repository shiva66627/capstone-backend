using CapStone.Data;
using CapStone.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CapStone.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CapstoneDbContext _context;

        public UserRepository(CapstoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
        }

        public async Task UpdateAsync(User entity)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == entity.UserId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.UserName = entity.UserName;
            user.MobileNumber = entity.MobileNumber;
            user.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Users.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
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

        public async Task<(IEnumerable<User> Data, int TotalCount)> GetFilteredUsersAsync(CapStone.DTO.User.UserFilterDTO filter)
        {
            // ->AsQueryable() allows u to query by filters
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Role))
            {
                if (Enum.TryParse<Role>(filter.Role, true, out var parsedRole))
                {
                    query = query.Where(u => u.Role == parsedRole);
                }
            }

            if (filter.IsActive.HasValue)
            {
                var statusToMatch = filter.IsActive.Value ? ActiveStatusEnum.Active : ActiveStatusEnum.InActive;
                query = query.Where(u => u.ActiveStatus == statusToMatch);
            }

            // Simple sorting logic
            if (filter.SortDirection.ToLower() == "asc")
            {
                query = filter.SortField.ToLower() switch
                {
                    "fullname" => query.OrderBy(u => u.UserName),
                    "email" => query.OrderBy(u => u.Email),
                    _ => query.OrderBy(u => u.CreatedDate)
                };
            }
            else
            {
                query = filter.SortField.ToLower() switch
                {
                    "fullname" => query.OrderByDescending(u => u.UserName),
                    "email" => query.OrderByDescending(u => u.Email),
                    _ => query.OrderByDescending(u => u.CreatedDate)
                };
            }

            return await query.GetPagedDataAsync(filter.PageNumber, filter.PageSize);
        }
    }
}