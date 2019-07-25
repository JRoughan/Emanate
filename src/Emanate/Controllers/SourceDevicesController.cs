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
    public class SourceDevicesController : ControllerBase
    {
        private readonly AdminDbContext _context;
        private readonly IHubContext<NotificationHub> hub;

        public SourceDevicesController(AdminDbContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            this.hub = hub;
        }

        // GET: api/SourceDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SourceDevice>>> GetSourceDevice()
        {
            return await _context.SourceDevices
                .Include(d => d.Type)
                .Include(d => d.Profile)
                .ToListAsync();
        }

        // GET: api/SourceDevices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SourceDevice>> GetSourceDevice(Guid id)
        {
            var sourceDevice = await _context.SourceDevices.FindAsync(id);

            if (sourceDevice == null)
            {
                return NotFound();
            }

            return sourceDevice;
        }

        // PUT: api/SourceDevices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSourceDevice(Guid id, SourceDevice sourceDevice)
        {
            if (id != sourceDevice.Id)
            {
                return BadRequest();
            }

            _context.Entry(sourceDevice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SourceDeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SourceDevices
        [HttpPost]
        public async Task<ActionResult<SourceDevice>> PostSourceDevice(SourceDevice sourceDevice)
        {
            _context.SourceDevices.Add(sourceDevice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSourceDevice", new { id = sourceDevice.Id }, sourceDevice);
        }

        // DELETE: api/SourceDevices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SourceDevice>> DeleteSourceDevice(Guid id)
        {
            var sourceDevice = await _context.SourceDevices.FindAsync(id);
            if (sourceDevice == null)
            {
                return NotFound();
            }

            _context.SourceDevices.Remove(sourceDevice);
            await _context.SaveChangesAsync();

            return sourceDevice;
        }

        private bool SourceDeviceExists(Guid id)
        {
            return _context.SourceDevices.Any(e => e.Id == id);
        }
    }
}
