using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebAPI.Controllers
{
    [EnableCors("*", "*", "*")]
    public class BaseAPRController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            decimal max = 5.00m;
            decimal min = 3.00m;
            Random random = new Random();
            decimal apr = (decimal)random.NextDouble() * (max - min) + min;
            return Ok(Math.Round(apr, 2));
        }
    }
}
