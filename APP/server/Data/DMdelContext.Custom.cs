using Microsoft.EntityFrameworkCore;
using Pes.Models;

namespace Pes.Data
{
    public partial class DMdelContext
    {
        partial void OnModelBuilding(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers");

            // Journal d'audit : nécessaire pour que l'AuditInterceptor puisse
            // écrire les entrées AuditLogs dans la même transaction SaveChanges.
            builder.Entity<AuditLog>();
        }
    }
}
