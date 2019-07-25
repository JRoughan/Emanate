using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Emanate.Web.Data;
using Emanate.Web.Model;
using Microsoft.AspNetCore.SignalR;

namespace Emanate.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourceDeviceTypesController : ControllerBase
    {
        private readonly AdminDbContext _context;
        private readonly IHubContext<NotificationHub> hub;

        public SourceDeviceTypesController(AdminDbContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            this.hub = hub;
        }

        // GET: api/SourceDeviceTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SourceDeviceType>>> GetSourceDeviceType()
        {
            return await _context.SourceDeviceTypes.ToListAsync();
        }

        // GET: api/SourceDeviceTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SourceDeviceType>> GetSourceDeviceType(Guid id)
        {
            var sourceDeviceType = await _context.SourceDeviceTypes.FindAsync(id);

            if (sourceDeviceType == null)
            {
                return NotFound();
            }

            return sourceDeviceType;
        }

        // PUT: api/SourceDeviceTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSourceDeviceType(Guid id, SourceDeviceType sourceDeviceType)
        {
            if (id != sourceDeviceType.Id)
            {
                return BadRequest();
            }

            _context.Entry(sourceDeviceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SourceDeviceTypeExists(id))
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

        // POST: api/SourceDeviceTypes
        [HttpPost]
        public async Task<ActionResult<SourceDeviceType>> PostSourceDeviceType(SourceDeviceType sourceDeviceType)
        {
            _context.SourceDeviceTypes.Add(sourceDeviceType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSourceDeviceType", new { id = sourceDeviceType.Id }, sourceDeviceType);
        }

        // DELETE: api/SourceDeviceTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SourceDeviceType>> DeleteSourceDeviceType(Guid id)
        {
            var sourceDeviceType = await _context.SourceDeviceTypes.FindAsync(id);
            if (sourceDeviceType == null)
            {
                return NotFound();
            }

            _context.SourceDeviceTypes.Remove(sourceDeviceType);
            await _context.SaveChangesAsync();

            return sourceDeviceType;
        }

        private bool SourceDeviceTypeExists(Guid id)
        {
            return _context.SourceDeviceTypes.Any(e => e.Id == id);
        }
    }
}
