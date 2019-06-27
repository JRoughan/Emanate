using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Emanate.Web.Controllers
{
    [Route("api/[controller]")]
    public class DisplayDevicesController : Controller
    {
        private readonly IHubContext<CounterHub> context;

        private static readonly List<DisplayDevice> devices = new List<DisplayDevice>
        {
            new DisplayDevice { Id = Guid.NewGuid(), Name = "Device 1" }, 
            new DisplayDevice { Id = Guid.NewGuid(), Name = "Device 2" }, 
            new DisplayDevice { Id = Guid.NewGuid(), Name = "Device 3" }, 
            new DisplayDevice { Id = Guid.NewGuid(), Name = "Device 4" }, 
            new DisplayDevice { Id = Guid.NewGuid(), Name = "Device 5" },
        };

        public class DisplayDevice
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public DisplayDevicesController(IHubContext<CounterHub> context)
        {
            this.context = context;
        }

        [HttpGet]
        public IEnumerable<DisplayDevice> Get()
        {
            return devices;
        }

        [HttpGet("{id}", Name = "Get")]
        public DisplayDevice Get(Guid id)
        {
            return devices.Single(d => d.Id == id);
        }

        [HttpPost]
        public async Task Post([FromBody] DisplayDevice displayDevice)
        {
            displayDevice.Id = Guid.NewGuid();
            devices.Add(displayDevice);
            await context.Clients.All.SendAsync("DisplayDeviceAdded", displayDevice);
        }

        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] DisplayDevice displayDevice)
        {
            devices[devices.FindIndex(d => d.Id == id)] = displayDevice;
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            devices.RemoveAll(d => d.Id == id);
            await context.Clients.All.SendAsync("DisplayDeviceRemoved", id);
        }
    }
}
