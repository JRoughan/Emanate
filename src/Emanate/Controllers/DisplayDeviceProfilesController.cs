﻿using System;
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
    public class DisplayDeviceProfilesController : ControllerBase
    {
        private readonly AdminDbContext db;
        private readonly IHubContext<NotificationHub> hub;

        public DisplayDeviceProfilesController(AdminDbContext db, IHubContext<NotificationHub> hub)
        {
            this.db = db;
            this.hub = hub;
        }

        // GET: api/DisplayDeviceProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisplayDeviceProfile>>> GetDisplayDeviceProfile()
        {
            return await db.DisplayDeviceProfiles
                //.Include(d => d.Type)
                .ToListAsync();
        }

        // GET: api/DisplayDeviceProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DisplayDeviceProfile>> GetDisplayDeviceProfile(Guid id)
        {
            var deviceProfile = await db.DisplayDeviceProfiles.FindAsync(id);

            if (deviceProfile == null)
            {
                return NotFound();
            }

            return deviceProfile;
        }

        // PUT: api/DisplayDeviceProfiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDisplayDeviceProfile(Guid id, DisplayDeviceProfile displayDeviceProfile)
        {
            if (id != displayDeviceProfile.Id)
            {
                return BadRequest();
            }

            db.Entry(displayDeviceProfile).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceProfileExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            await hub.Clients.All.SendAsync("DisplayDeviceProfileUpdated", displayDeviceProfile);

            return NoContent();
        }

        // POST: api/DisplayDeviceProfiles
        [HttpPost]
        public async Task<ActionResult<DisplayDeviceProfile>> PostDisplayDeviceProfile(DisplayDeviceProfile displayDeviceProfile)
        {
            db.DisplayDeviceProfiles.Add(displayDeviceProfile);
            await db.SaveChangesAsync();

            await hub.Clients.All.SendAsync("DisplayDeviceProfileAdded", displayDeviceProfile);

            return CreatedAtAction("GetDisplayDeviceProfile", new { id = displayDeviceProfile.Id }, displayDeviceProfile);
        }

        // DELETE: api/DisplayDeviceProfiles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DisplayDeviceProfile>> DeleteDisplayDeviceProfile(Guid id)
        {
            var deviceProfile = await db.DisplayDeviceProfiles.FindAsync(id);
            if (deviceProfile == null)
            {
                return NotFound();
            }

            db.DisplayDeviceProfiles.Remove(deviceProfile);
            await db.SaveChangesAsync();

            await hub.Clients.All.SendAsync("DisplayDeviceProfileRemoved", deviceProfile);

            return deviceProfile;
        }

        private bool DeviceProfileExists(Guid id)
        {
            return db.DisplayDeviceProfiles.Any(e => e.Id == id);
        }
    }
}