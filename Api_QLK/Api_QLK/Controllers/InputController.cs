using Api_QLK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_QLK.Controllers
{
    public class InputController : ApiController
    {

        Input[] inputApi = new Input[]
        {
            new Input{ Id ="001", InputName = "vải", Count =100},
            new Input{ Id ="002", InputName = "sắt", Count =10},
            new Input{ Id ="003", InputName = "trái cây", Count =20}
        };

        
        // GET: api/Input
        public HttpResponseMessage Get()
        {
            
            return Request.CreateResponse(HttpStatusCode.OK,inputApi);
        }

        // GET: api/Input/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Input
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Input/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Input/5
        public void Delete(int id)
        {
        }
    }
}
