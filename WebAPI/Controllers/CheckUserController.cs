using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [EnableCors("*","*","*")]
    public class CheckUserController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            UserLoginDB db = new UserLoginDB();

            try
            {
                var data = (from a in db.UserLogins where a.UserName.Equals(id) select a).FirstOrDefault();

                if(data != null)
                {
                    return BadRequest("Contact Exist!");
                }
                else
                {
                    return Ok("Success");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]UserLogin login)
        {
            UserLoginDB db = new UserLoginDB();

            UserLogin userLogin = (from a in db.UserLogins where a.UserName.Equals(login.UserName) select a).FirstOrDefault();


            if (userLogin != null)
            {

                if (userLogin.Password.Equals(login.Password))
                {
                    return Ok(userLogin.GUID);
                }
                else
                {
                    return BadRequest("Invalid Username or Password");
                }
            }
            else
            {
                return BadRequest("Invalid Username or Password");
            }
        }


    }
}
