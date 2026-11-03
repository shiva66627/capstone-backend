using Microsoft.EntityFrameworkCore;

namespace CapStone.Repositories
{
    public static class QueryableExtensions
    {
        public static async Task<(IEnumerable<T> Data, int TotalRecords)> GetPagedDataAsync<T>(
            this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalRecords = await query.CountAsync();
            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (data, totalRecords);
        }
    }
}
