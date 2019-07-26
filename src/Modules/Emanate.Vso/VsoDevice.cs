using Emanate.Model;

namespace Emanate.Vso
{
    public class VsoDevice : SourceDevice
    {
        public string Uri { get; set; }
        public int PollingInterval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}