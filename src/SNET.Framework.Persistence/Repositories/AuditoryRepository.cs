using Microsoft.EntityFrameworkCore;
using SNET.Framework.Domain.Entities;
using SNET.Framework.Domain.Repositories;

namespace SNET.Framework.Persistence.Repositories
{
    public class AuditoryRepository : GenericRepository<Auditory>, IAuditoryRepository
    {
        public AuditoryRepository(ApiDbContext context) : base(context)
        {
        }
        public async Task<Auditory> GetByIdAsync(Guid auditoryId)
        {
            return await _context.Set<Auditory>().FindAsync(auditoryId);
        }

        public async Task<List<Auditory>> GetAllAsync()
        {
            return await _context.Set<Auditory>().ToListAsync();
        }

        public async Task<List<Auditory>> GetByTenantIdAsync(string tenantId)
        {
            return await _context.Set<Auditory>()
                .Where(a => a.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<List<Auditory>> GetByUserIdAsync(string userId)
        {
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                return await _context.Set<Auditory>()
                    .Where(a => a.UserId == userGuid)  // Compara con Guid
                    .ToListAsync();
            }

            return new List<Auditory>();  // Retorna una lista vacía si el string no es un Guid válido
        }


        public async Task<List<Auditory>> GetByLevelIdAsync(int levelId)
        {
            return await _context.Set<Auditory>()
                .Where(a => a.LevelId == levelId)
                .ToListAsync();
        }

        public async Task<List<Auditory>> GetByCrudOperationIdAsync(int crudOperationId)
        {
            return await _context.Set<Auditory>()
                .Where(a => a.CrudOperationId == crudOperationId)
                .ToListAsync();
        }

        public async Task<Auditory> CreateAsync(Auditory auditory)
        {
            await _context.Set<Auditory>().AddAsync(auditory);
            await _context.SaveChangesAsync();
            return auditory;
        }

        public async Task UpdateAsync(Auditory auditory)
        {
            _context.Set<Auditory>().Update(auditory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(Guid auditoryId)
        {
            var auditory = await _context.Set<Auditory>().FindAsync(auditoryId);
            if (auditory != null)
            {
                _context.Set<Auditory>().Remove(auditory);
                await _context.SaveChangesAsync();
            }
        }
    }
}
