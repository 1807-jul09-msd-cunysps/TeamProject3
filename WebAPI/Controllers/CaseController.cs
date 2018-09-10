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
    public class CaseController : ApiController
    {
        CrmServiceClient client = new CrmServiceClient("Url=https://cunyraveturetraining.crm.dynamics.com; Username=systema@revaturefinalproj.onmicrosoft.com; Password=Nut65215; AuthType=Office365");

        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            QueryExpression query = new QueryExpression();
            query.EntityName = "incident";
            query.ColumnSet = new ColumnSet(new string[] { "title", "ticketnumber", "description", "statuscode" });
            query.Criteria.AddCondition("customerid", ConditionOperator.Equal, id);

            try
            {
                EntityCollection incidents = service.RetrieveMultiple(query);
                List<object> list = new List<object>();
                foreach (Entity item in incidents.Entities)
                {
                    var incident = new
                    {
                        ticketnumber = item.Attributes["ticketnumber"],
                        title = item.Attributes["title"],
                        description = item.Attributes["description"],
                        statuscode = item.Attributes["statuscode"]

                    };
                    list.Add(incident);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]IncidentCase incidentCase)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            try
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = "revfinal_mortgage";
                query.ColumnSet = new ColumnSet(new string[] { "revfinal_name" });
                query.Criteria.AddCondition("revfinal_mortgagenumber", ConditionOperator.Equal, incidentCase.MortgageNumber);
                Entity mortgage = service.RetrieveMultiple(query).Entities.FirstOrDefault();

                Entity incident = new Entity("incident");
                incident.Attributes.Add("customerid", new EntityReference("contact", new Guid(incidentCase.ContactID)));
                incident.Attributes.Add("title", incidentCase.Subject);
                incident.Attributes.Add("description", incidentCase.Description);
                incident.Attributes.Add("prioritycode", new OptionSetValue(incidentCase.Priority));
                incident.Attributes.Add("revfinal_highpriorityreason", incidentCase.HighReason);
                incident.Attributes.Add("revfinal_type", new OptionSetValue(incidentCase.Type));
                incident.Attributes.Add("revfinal_mortgage", new EntityReference(mortgage.LogicalName, mortgage.Id));

                service.Create(incident);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }

    public class IncidentCase
    {
        public string ContactID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public string HighReason { get; set; }
        public int Type { get; set; }
        public string MortgageNumber { get; set; }
    }
}
