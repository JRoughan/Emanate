using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Emanate.Core.Configuration;

namespace Emanate.Api.Controllers
{
    public class OutputGroupsController : ApiController
    {
        // GET api/values
        public IEnumerable<Mapping> Get()
        {
            return Store.Config.Mappings;
        }

        // GET api/values/5
        public Mapping Get(Guid id)
        {
            return Store.Config.Mappings.SingleOrDefault(og => og.OutputDeviceId == id);
        }

        //// POST api/values
        //public void Post([FromBody]string value)
        //{
        //    Store.Config.OutputGroups.
        //}

        // PUT api/values/5
        public void Put(int id, Mapping value)
        {
            Store.Config.Mappings.Add(value);
        }

        // DELETE api/values/5
        public void Delete(Guid id)
        {
            Store.Config.Mappings.RemoveAll(og => og.OutputDeviceId == id);
        }
    }
}
