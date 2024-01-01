using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using MongoDbOrdersAPI.Model;

namespace Major_BeanScene_ReservationSystem.Services
{
    public class ApplicationContextAPIService
    {
        private readonly ApplicationDbContext _context;
        public ApplicationContextAPIService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Table?> GetTableByNameAsync(string tableName) =>
            await _context.Tables.Where(t => t.Name == tableName).FirstOrDefaultAsync();

        public async Task<Staff?> GetStaffAsync(string userId) =>
            await _context.Staffs.Where(s => s.UserId == userId).FirstOrDefaultAsync();
    }
}
