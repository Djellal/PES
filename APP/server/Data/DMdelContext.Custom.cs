using Microsoft.EntityFrameworkCore;
using Pes.Models;

namespace Pes.Data
{
    public partial class DMdelContext
    {
        partial void OnModelBuilding(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
        }
    }
}
