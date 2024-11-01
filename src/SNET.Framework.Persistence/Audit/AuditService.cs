using SNET.Framework.Domain.Audit;

namespace SNET.Framework.Persistence.Audit
{
    public class AuditService : IAuditService
    {
        private readonly ApiDbContext _context;
        public AuditService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task LogCreateAsync(AuditLog auditLog)
        {
            await LogActionAsync(auditLog, "Create");
        }

        public async Task LogUpdateAsync(AuditLog auditLog)
        {
            await LogActionAsync(auditLog, "Update");
        }

        public async Task LogDeleteAsync(AuditLog auditLog)
        {
            await LogActionAsync(auditLog, "Delete");
        }

        private async Task LogActionAsync(AuditLog auditLog, string actionType)
        {
            auditLog.Action = actionType; // Establecer tipo de acción
            _context.Set<AuditLog>().Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }

}
