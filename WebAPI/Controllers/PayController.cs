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

namespace WebAPI.Controllers
{
    [EnableCors("*", "*", "*")]
    public class PayController : ApiController
    {
        CrmServiceClient client = new CrmServiceClient("Url=https://cunyraveturetraining.crm.dynamics.com; Username=systema@revaturefinalproj.onmicrosoft.com; Password=Nut65215; AuthType=Office365");

        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            Entity payment = new Entity("revfinal_paymentrecord", new Guid(id));
            payment.Attributes.Add("revfinal_paymentstatus", new OptionSetValue(273250001));

            try
            {
                service.Update(payment);
                return Ok("Success");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
