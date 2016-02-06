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
        public IEnumerable<OutputGroup> Get()
        {
            return Store.Config.OutputGroups;
        }

        // GET api/values/5
        public OutputGroup Get(Guid id)
        {
            return Store.Config.OutputGroups.SingleOrDefault(og => og.Id == id);
        }

        //// POST api/values
        //public void Post([FromBody]string value)
        //{
        //    Store.Config.OutputGroups.
        //}

        // PUT api/values/5
        public void Put(int id, OutputGroup value)
        {
            Store.Config.OutputGroups.Add(value);
        }

        // DELETE api/values/5
        public void Delete(Guid id)
        {
            Store.Config.OutputGroups.RemoveAll(og => og.Id == id);
        }
    }
}
