using Emanate.Web.Model;
using Microsoft.EntityFrameworkCore;

namespace Emanate.Web.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options) { }

        public DbSet<DisplayDevice> DisplayDevices { get; set; }

        public DbSet<DisplayDeviceProfile> DisplayDeviceProfiles { get; set; }
    }
}
