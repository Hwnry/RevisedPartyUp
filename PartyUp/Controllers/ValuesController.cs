using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PartyUp.Controllers
{
    /***
     * This is a controller that was used to test the network connectivity
     * by creating a controller for each of the RESTful calls.
     * 
     * Get simply get simply returns a couple of test values.
     * 
     * Get with a value specified in the url returns specific data.
     * 
     * Post was a test to ensure posting a value was possible.
     * 
     * Put was used to test if a put request was possible
     * 
     * Delete was used to test if the api could handle
     * delete requests.
     */
    [Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            //return values
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            //specific return value
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            //break points were set here to ensure controller was called
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
            //break points were set here to ensure controller was called
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            //break points were set here to ensure controller was called
        }
    }
}
