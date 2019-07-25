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
            var defaultProfile = AddIfNew(context.DisplayDeviceProfiles, new DisplayDeviceProfile { Name = "Em-Default" });
            AddIfNew(context.DisplayDeviceType, new DisplayDeviceType { Name = "Emanate", Icon = "Emanate.png", Profiles = new List<DisplayDeviceProfile>{ defaultProfile } });

            defaultProfile = AddIfNew(context.DisplayDeviceProfiles, new DisplayDeviceProfile { Name = "Del-Default" });
            var otherProfile = AddIfNew(context.DisplayDeviceProfiles, new DisplayDeviceProfile { Name = "Del-Other" });
            AddIfNew(context.DisplayDeviceType, new DisplayDeviceType { Name = "Delcom", Icon = "Delcom.jpg", Profiles = new List<DisplayDeviceProfile> { defaultProfile, otherProfile } });

            var defaultSourceProfile = AddIfNew(context.SourceDeviceProfiles, new SourceDeviceProfile { Name = "TC-Default" });
            AddIfNew(context.SourceDeviceTypes, new SourceDeviceType { Name = "TeamCity", Icon = "TeamCity.svg", Profiles = new List<SourceDeviceProfile> { defaultSourceProfile } });

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
            // Device Types
            var testDisplayProfile = AddIfNew(context.DisplayDeviceProfiles, new DisplayDeviceProfile { Name = "<<Test-Display-Default>>" });
            var testDisplayDeviceType = new DisplayDeviceType { Name = "<<Test-Display-Type>>", Icon = "Test-Display.png", Profiles = new List<DisplayDeviceProfile> { testDisplayProfile } };
            AddIfNew(context.DisplayDeviceType, testDisplayDeviceType);

            var testSourceProfile = AddIfNew(context.SourceDeviceProfiles, new SourceDeviceProfile { Name = "<<Test-Source-Default>>" });
            var testSourceDeviceType = new SourceDeviceType { Name = "<<Test-Source-Type>>", Icon = "Test-Source.png", Profiles = new List<SourceDeviceProfile> { testSourceProfile } };
            AddIfNew(context.SourceDeviceTypes, testSourceDeviceType);

            // Devices
            var testSourceDevice = new SourceDevice { Name = "<<Test-Source>>", Type = testSourceDeviceType, Profile = testSourceProfile };
            AddIfNew(context.SourceDevices, testSourceDevice);

            var testDisplayDevice = new DisplayDevice { Name = "<<Test-Display>>", Type = testDisplayDeviceType, Profile = testDisplayProfile };
            AddIfNew(context.DisplayDevices, testDisplayDevice);

            var displayConfig = new DisplayConfiguration
            {
                DisplayDevice = testDisplayDevice,
                SourceGroups = new List<SourceGroup>
                {
                    new SourceGroup
                    {
                        SourceDevice = testSourceDevice,
                        SourceConfiguration = new List<SourceConfiguration>
                        {
                            new SourceConfiguration
                            {
                                Builds = "Build1"
                            }
                        }
                    }
                }
            };
            AddIfNew(context.DisplayConfigurations, displayConfig);

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
