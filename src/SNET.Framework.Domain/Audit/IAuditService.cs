namespace SNET.Framework.Domain.Audit
{
    public interface IAuditService
    {
        Task LogCreateAsync(AuditLog auditLog);
        Task LogUpdateAsync(AuditLog auditLog);
        Task LogDeleteAsync(AuditLog auditLog);
    }
}
