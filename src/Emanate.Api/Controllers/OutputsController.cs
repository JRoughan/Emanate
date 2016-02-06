using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Emanate.Core.Output;

namespace Emanate.Api.Controllers
{
    public class OutputsController : ApiController
    {
        // GET api/values
        public IEnumerable<IOutputDevice> Get()
        {
            return Store.Config.OutputDevices;
        }

        // GET api/values/5
        public IOutputDevice Get(Guid id)
        {
            return Store.Config.OutputDevices.SingleOrDefault(og => og.Id == id);
        }

        //// POST api/values
        //public void Post([FromBody]string value)
        //{
        //    Store.Config.OutputGroups.
        //}

        // PUT api/values/5
        public void Put(int id, IOutputDevice value)
        {
            Store.Config.OutputDevices.Add(value);
        }

        // DELETE api/values/5
        public void Delete(Guid id)
        {
            Store.Config.OutputDevices.RemoveAll(og => og.Id == id);
        }
    }
}
