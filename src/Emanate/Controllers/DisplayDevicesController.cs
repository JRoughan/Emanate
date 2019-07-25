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
    public class DisplayDevicesController : ControllerBase
    {
        private readonly AdminDbContext db;
        private readonly IHubContext<NotificationHub> hub;

        public DisplayDevicesController(AdminDbContext db, IHubContext<NotificationHub> hub)
        {
            this.db = db;
            this.hub = hub;
        }

        // GET: api/DisplayDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisplayDevice>>> GetDisplayDevices()
        {
            return await db.DisplayDevices
                .Include(d => d.Type)
                .Include(d => d.Profile)
                .ToListAsync();
        }

        // GET: api/DisplayDevices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DisplayDevice>> GetDisplayDevice(Guid id)
        {
            var displayDevice = await db.DisplayDevices.FindAsync(id);

            if (displayDevice == null)
            {
                return NotFound();
            }

            return displayDevice;
        }

        // PUT: api/DisplayDevices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDisplayDevice(Guid id, DisplayDevice displayDevice)
        {
            if (id != displayDevice.Id)
            {
                return BadRequest();
            }

            db.Entry(displayDevice).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DisplayDeviceExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            await hub.Clients.All.SendAsync("DisplayDeviceUpdated", displayDevice);

            return NoContent();
        }

        // POST: api/DisplayDevices
        [HttpPost]
        public async Task<ActionResult<DisplayDevice>> PostDisplayDevice(DisplayDevice displayDevice)
        {
            db.DisplayDevices.Add(displayDevice);
            await db.SaveChangesAsync();

            await hub.Clients.All.SendAsync("DisplayDeviceAdded", displayDevice);

            return CreatedAtAction("GetDisplayDevice", new { id = displayDevice.Id }, displayDevice);
        }

        // DELETE: api/DisplayDevices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DisplayDevice>> DeleteDisplayDevice(Guid id)
        {
            var displayDevice = await db.DisplayDevices.FindAsync(id);
            if (displayDevice == null)
            {
                return NotFound();
            }

            db.DisplayDevices.Remove(displayDevice);
            await db.SaveChangesAsync();

            await hub.Clients.All.SendAsync("DisplayDeviceRemoved", id);

            return displayDevice;
        }

        private bool DisplayDeviceExists(Guid id)
        {
            return db.DisplayDevices.Any(e => e.Id == id);
        }
    }
}
