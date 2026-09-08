using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class AdminRepository : GenericRepository<Domain.Entities.Admin>, IAdminRepository
    {
        public AdminRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Domain.Entities.Admin?> GetByEmailAsync(string email)
        {
            return await _dbContext.Admins.FirstOrDefaultAsync(a => a.Email == email);
        }
    }
}
