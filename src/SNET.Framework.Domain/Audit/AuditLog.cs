using SNET.Framework.Domain.Entities;

namespace SNET.Framework.Domain.Audit
{
    public class AuditLog
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string TableName { get; set; }
        public string Action { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime Timestamp { get; set; }
        public string? ChangeReason { get; set; }
        public User User { get; set; }
    }
}
