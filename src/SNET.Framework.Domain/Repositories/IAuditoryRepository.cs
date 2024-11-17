using SNET.Framework.Domain.Entities;

namespace SNET.Framework.Domain.Repositories
{
    public interface IAuditoryRepository: IGenericRepository<Auditory>
    {
        Task<Auditory> GetByIdAsync(Guid auditoryId);
        Task<List<Auditory>> GetAllAsync();
        Task<List<Auditory>> GetByTenantIdAsync(string tenantId);
        Task<List<Auditory>> GetByUserIdAsync(string userId);
        Task<List<Auditory>> GetByLevelIdAsync(int levelId);
        Task<List<Auditory>> GetByCrudOperationIdAsync(int crudOperationId);
        Task<Auditory> CreateAsync(Auditory auditory);
        Task UpdateAsync(Auditory auditory);
        Task DeleteByIdAsync(Guid auditoryId);
    }
}