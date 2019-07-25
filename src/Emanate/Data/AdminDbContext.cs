using System.Linq;
using System.Threading.Tasks;
using Emanate.Model;
using Microsoft.EntityFrameworkCore;

namespace Emanate.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options) { }

        public DbSet<DisplayDevice> DisplayDevices { get; set; }

        public DbSet<DisplayDeviceProfile> DisplayDeviceProfiles { get; set; }

        public DbSet<DisplayDeviceType> DisplayDeviceType { get; set; }

        public DbSet<SourceDeviceProfile> SourceDeviceProfiles { get; set; }

        public DbSet<SourceDeviceType> SourceDeviceTypes { get; set; }

        public DbSet<SourceDevice> SourceDevices { get; set; }

        public DbSet<DisplayConfiguration> DisplayConfigurations { get; set; }
    }
}
