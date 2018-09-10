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
    public class PaymentController : ApiController
    {
        CrmServiceClient client = new CrmServiceClient("Url=https://cunyraveturetraining.crm.dynamics.com; Username=systema@revaturefinalproj.onmicrosoft.com; Password=Nut65215; AuthType=Office365");

        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            string xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='revfinal_paymentrecord'>
    <attribute name='revfinal_paymentrecordid' />
    <attribute name='revfinal_name' />
    <attribute name='revfinal_paymentstatus' />
    <attribute name='revfinal_payment' />
    <attribute name='revfinal_duedate' />
    <order attribute='revfinal_duedate' descending='true' />
    <link-entity name='revfinal_mortgage' from='revfinal_mortgageid' to='revfinal_mortgagerequestid' link-type='inner' alias='mortgage'>
      <attribute name='revfinal_mortgagenumber' />
      <filter type='and'>
        <condition attribute='revfinal_contact' operator='eq' uitype='contact' value='{2bereplaced}' />
      </filter>
    </link-entity>
  </entity>
</fetch>";

            xml = xml.Replace("2bereplaced", id);

            try
            {
                EntityCollection payments = service.RetrieveMultiple(new FetchExpression(xml));
                List<object> list = new List<object>();
                foreach (Entity item in payments.Entities)
                {
                    var payment = new
                    {
                        id = item.Attributes["revfinal_paymentrecordid"],
                        revfinal_name = item.Attributes["revfinal_name"],
                        revfinal_mortgagenumber = item.Attributes["mortgage.revfinal_mortgagenumber"],
                        revfinal_duedate = item.Attributes["revfinal_duedate"],
                        revfinal_payment = item.Attributes["revfinal_payment"],
                        revfinal_paymentstatus = item.Attributes["revfinal_paymentstatus"]
                    };
                    list.Add(payment);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }


        }
    }
}
