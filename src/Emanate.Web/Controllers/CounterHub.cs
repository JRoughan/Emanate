using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Emanate.Web.Controllers
{
    public class CounterHub : Hub
    {
        public Task IncrementCounter()
        {
            return Clients.All.SendAsync("IncrementCounter");
        }

        public Task DecrementCounter()
        {
            return Clients.All.SendAsync("DecrementCounter");
        }
    }
}
