using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Emanate.Web.Data;
using Emanate.Web.Model;
using Microsoft.AspNetCore.SignalR;

namespace Emanate.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisplayConfigurationsController : ControllerBase
    {
        private readonly AdminDbContext db;
        private readonly IHubContext<NotificationHub> hub;

        public DisplayConfigurationsController(AdminDbContext db, IHubContext<NotificationHub> hub)
        {
            this.db = db;
            this.hub = hub;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisplayConfiguration>>> GetDisplayConfigurations()
        {
            return await db.DisplayConfigurations
                .Include(c => c.DisplayDevice)
                .Include(c => c.SourceGroups)
                    .ThenInclude(g => g.SourceDevice)
                .Include(c => c.SourceGroups)
                    .ThenInclude(g => g.SourceConfiguration)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DisplayConfiguration>> GetDisplayConfiguration(Guid id)
        {
            var displayConfiguration = await db.DisplayConfigurations.FindAsync(id);

            if (displayConfiguration == null)
            {
                return NotFound();
            }

            return displayConfiguration;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDisplayConfiguration(Guid id, DisplayConfiguration displayConfiguration)
        {
            if (id != displayConfiguration.Id)
            {
                return BadRequest();
            }

            db.Entry(displayConfiguration).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DisplayConfigurationExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            await hub.Clients.All.SendAsync("DisplayConfigurationUpdated", displayConfiguration);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<DisplayConfiguration>> PostDisplayConfiguration(DisplayConfiguration displayConfiguration)
        {
            db.DisplayConfigurations.Add(displayConfiguration);
            await db.SaveChangesAsync();

            await hub.Clients.All.SendAsync("DisplayConfigurationAdded", displayConfiguration);

            return CreatedAtAction("GetDisplayConfiguration", new { id = displayConfiguration.Id }, displayConfiguration);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DisplayConfiguration>> DeleteDisplayConfiguration(Guid id)
        {
            var displayConfiguration = await db.DisplayConfigurations.FindAsync(id);
            if (displayConfiguration == null)
            {
                return NotFound();
            }

            db.DisplayConfigurations.Remove(displayConfiguration);
            await db.SaveChangesAsync();

            await hub.Clients.All.SendAsync("DisplayConfigurationRemoved", id);

            return displayConfiguration;
        }

        private bool DisplayConfigurationExists(Guid id)
        {
            return db.DisplayConfigurations.Any(e => e.Id == id);
        }
    }
}
