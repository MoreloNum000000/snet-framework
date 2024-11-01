using Microsoft.EntityFrameworkCore;
using SNET.Framework.Domain.Audit;

namespace SNET.Framework.Persistence;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApiDbContext).Assembly);
    }
}
