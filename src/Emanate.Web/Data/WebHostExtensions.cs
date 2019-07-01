using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Emanate.Web.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Emanate.Web.Data
{
    public static class WebHostExtensions
    {
        public static IWebHost SeedData(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<AdminDbContext>();

                DeleteTestDatabase(context);

                context.Database.Migrate();

                SeedProductionData(context);
                SeedTestData(context);
            }

            return host;
        }

        private static void SeedProductionData(AdminDbContext context)
        {
            var defaultProfile = AddIfNew(context.DisplayDeviceProfiles, new DisplayDeviceProfile { Name = "Default" });
            AddIfNew(context.DisplayDeviceType, new DisplayDeviceType { Name = "Web", Icon = "Web.png", Profiles = new List<DisplayDeviceProfile>{ defaultProfile } });

            defaultProfile = AddIfNew(context.DisplayDeviceProfiles, new DisplayDeviceProfile { Name = "Default" });
            AddIfNew(context.DisplayDeviceType, new DisplayDeviceType { Name = "Delcom", Icon = "Delcom.png", Profiles = new List<DisplayDeviceProfile> { defaultProfile } });

            context.SaveChanges(true);
        }

        [Conditional("DEBUG")]
        private static void DeleteTestDatabase(AdminDbContext context)
        {
            context.Database.EnsureDeleted();
        }

        [Conditional("DEBUG")]
        private static void SeedTestData(AdminDbContext context)
        {
            var type = context.DisplayDeviceType.Single(d => d.Name == "Web");
            AddIfNew(context.DisplayDevices, new DisplayDevice { Name = "Web Dashboard 1", Type = type, Profile = type.Profiles.Single() });
            AddIfNew(context.DisplayDevices, new DisplayDevice { Name = "Web Dashboard 2", Type = type, Profile = type.Profiles.Single() });
            AddIfNew(context.DisplayDevices, new DisplayDevice { Name = "Web Dashboard 3", Type = type, Profile = type.Profiles.Single() });

            type = context.DisplayDeviceType.Single(d => d.Name == "Delcom");
            AddIfNew(context.DisplayDevices, new DisplayDevice { Name = "Delcom Dashboard 1", Type = type, Profile = type.Profiles.Single() });

            context.SaveChanges(true);
        }



        private static T AddIfNew<T>(DbSet<T> dbSet, T item)
            where T : Entity
        {
            var existingItem = dbSet.FirstOrDefault(p => p.Id == item.Id);
            if (existingItem == null)
            {
                dbSet.Add(item);
            }

            return existingItem ?? item;
        }
    }
}
