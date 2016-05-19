using System;

namespace Emanate.Vso
{
    public class VsoDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public int PollingInterval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}