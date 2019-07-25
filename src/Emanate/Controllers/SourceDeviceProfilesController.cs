using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emanate.Data;
using Emanate.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Emanate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourceDeviceProfilesController : ControllerBase
    {
        private readonly AdminDbContext _context;
        private readonly IHubContext<NotificationHub> hub;

        public SourceDeviceProfilesController(AdminDbContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            this.hub = hub;
        }

        // GET: api/SourceDeviceProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SourceDeviceProfile>>> GetSourceDeviceProfile()
        {
            return await _context.SourceDeviceProfiles.ToListAsync();
        }

        // GET: api/SourceDeviceProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SourceDeviceProfile>> GetSourceDeviceProfile(Guid id)
        {
            var sourceDeviceProfile = await _context.SourceDeviceProfiles.FindAsync(id);

            if (sourceDeviceProfile == null)
            {
                return NotFound();
            }

            return sourceDeviceProfile;
        }

        // PUT: api/SourceDeviceProfiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSourceDeviceProfile(Guid id, SourceDeviceProfile sourceDeviceProfile)
        {
            if (id != sourceDeviceProfile.Id)
            {
                return BadRequest();
            }

            _context.Entry(sourceDeviceProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SourceDeviceProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await hub.Clients.All.SendAsync("SourceDeviceProfileUpdated", sourceDeviceProfile);

            return NoContent();
        }

        // POST: api/SourceDeviceProfiles
        [HttpPost]
        public async Task<ActionResult<SourceDeviceProfile>> PostSourceDeviceProfile(SourceDeviceProfile sourceDeviceProfile)
        {
            _context.SourceDeviceProfiles.Add(sourceDeviceProfile);
            await _context.SaveChangesAsync();

            await hub.Clients.All.SendAsync("SourceDeviceProfileUpdated", sourceDeviceProfile);

            return CreatedAtAction("GetSourceDeviceProfile", new { id = sourceDeviceProfile.Id }, sourceDeviceProfile);
        }

        // DELETE: api/SourceDeviceProfiles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SourceDeviceProfile>> DeleteSourceDeviceProfile(Guid id)
        {
            var sourceDeviceProfile = await _context.SourceDeviceProfiles.FindAsync(id);
            if (sourceDeviceProfile == null)
            {
                return NotFound();
            }

            _context.SourceDeviceProfiles.Remove(sourceDeviceProfile);
            await _context.SaveChangesAsync();

            await hub.Clients.All.SendAsync("SourceDeviceProfileUpdated", sourceDeviceProfile);

            return sourceDeviceProfile;
        }

        private bool SourceDeviceProfileExists(Guid id)
        {
            return _context.SourceDeviceProfiles.Any(e => e.Id == id);
        }
    }
}
