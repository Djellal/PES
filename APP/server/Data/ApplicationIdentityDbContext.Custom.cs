using Microsoft.EntityFrameworkCore;
using Pes.Models;

namespace Pes.Data
{
    public partial class ApplicationIdentityDbContext
    {
        partial void OnModelBuilding(ModelBuilder builder)
        {
            builder.Entity<AuditLog>().HasIndex(a => a.Timestamp);
            builder.Entity<AuditLog>().HasIndex(a => a.UserId);
            builder.Entity<AuditLog>().HasIndex(a => a.Action);
            builder.Entity<AuditLog>().HasIndex(a => new { a.EntityType, a.EntityId });
        }
    }
}
