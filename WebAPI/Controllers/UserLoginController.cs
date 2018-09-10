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
    [EnableCors("*", "*", "*")]
    public class UserLoginController : ApiController
    {
        CrmServiceClient client = new CrmServiceClient("Url=https://revaturefinalproj.crm.dynamics.com; Username=systema@revaturefinalproj.onmicrosoft.com; Password=Nut65215; AuthType=Office365");

        [HttpPost]
        public IHttpActionResult Post([FromBody]UserLogin_Web user)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            QueryExpression query = new QueryExpression();
            query.EntityName = "revfinal_mortgage";
            query.ColumnSet = new ColumnSet(new string[] { "revfinal_contact" });
            query.Criteria.AddCondition("revfinal_mortgagenumber", ConditionOperator.Equal, user.MortgageNumber);
            Entity mortgage = service.RetrieveMultiple(query).Entities.FirstOrDefault();

            UserLogin login = new UserLogin
            {
                UserName = user.UserName,
                Password = user.Password,
                GUID = mortgage.GetAttributeValue<EntityReference>("revfinal_contact").Id.ToString()
            };

            try
            {
                UserLoginDB db = new UserLoginDB();

                db.UserLogins.Add(login);
                db.SaveChanges();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }

    public class UserLogin_Web
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MortgageNumber { get; set; }
    }
}
