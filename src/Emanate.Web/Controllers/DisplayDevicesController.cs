using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Emanate.Web.Controllers
{
    [Route("api/[controller]")]
    public class DisplayDevicesController : Controller
    {
        private static readonly DisplayDevice[] devices =
        {
            new DisplayDevice { Name = "Device 1" }, 
            new DisplayDevice { Name = "Device 2" }, 
            new DisplayDevice { Name = "Device 3" }, 
            new DisplayDevice { Name = "Device 4" }, 
            new DisplayDevice { Name = "Device 5" },
        };

        public IEnumerable<DisplayDevice> Get()
        {
            return devices;
        }

        public class DisplayDevice
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}
