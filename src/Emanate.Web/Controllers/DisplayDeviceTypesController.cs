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
    public class DisplayDeviceTypesController : ControllerBase
    {
        private readonly AdminDbContext _context;
        private readonly IHubContext<NotificationHub> hub;

        public DisplayDeviceTypesController(AdminDbContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            this.hub = hub;
        }

        // GET: api/DisplayDeviceTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisplayDeviceType>>> GetDisplayDeviceType()
        {
            return await _context.DisplayDeviceType.ToListAsync();
        }

        // GET: api/DisplayDeviceTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DisplayDeviceType>> GetDisplayDeviceType(Guid id)
        {
            var displayDeviceType = await _context.DisplayDeviceType.FindAsync(id);

            if (displayDeviceType == null)
            {
                return NotFound();
            }

            return displayDeviceType;
        }

        // PUT: api/DisplayDeviceTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDisplayDeviceType(Guid id, DisplayDeviceType displayDeviceType)
        {
            if (id != displayDeviceType.Id)
            {
                return BadRequest();
            }

            _context.Entry(displayDeviceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DisplayDeviceTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await hub.Clients.All.SendAsync("DisplayDeviceUpdated", displayDeviceType);

            return NoContent();
        }

        // POST: api/DisplayDeviceTypes
        [HttpPost]
        public async Task<ActionResult<DisplayDeviceType>> PostDisplayDeviceType(DisplayDeviceType displayDeviceType)
        {
            _context.DisplayDeviceType.Add(displayDeviceType);
            await _context.SaveChangesAsync();

            await hub.Clients.All.SendAsync("DisplayDeviceTypeAdded", displayDeviceType);

            return CreatedAtAction("GetDisplayDeviceType", new { id = displayDeviceType.Id }, displayDeviceType);
        }

        // DELETE: api/DisplayDeviceTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DisplayDeviceType>> DeleteDisplayDeviceType(Guid id)
        {
            var displayDeviceType = await _context.DisplayDeviceType.FindAsync(id);
            if (displayDeviceType == null)
            {
                return NotFound();
            }

            _context.DisplayDeviceType.Remove(displayDeviceType);
            await _context.SaveChangesAsync();

            await hub.Clients.All.SendAsync("DisplayDeviceTypeRemoved", displayDeviceType);

            return displayDeviceType;
        }

        private bool DisplayDeviceTypeExists(Guid id)
        {
            return _context.DisplayDeviceType.Any(e => e.Id == id);
        }
    }
}
