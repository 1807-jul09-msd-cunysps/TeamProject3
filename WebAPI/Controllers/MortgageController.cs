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
    public class MortgageController : ApiController
    {
        CrmServiceClient client = new CrmServiceClient("Url=https://cunyraveturetraining.crm.dynamics.com; Username=systema@revaturefinalproj.onmicrosoft.com; Password=Nut65215; AuthType=Office365");

        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            QueryExpression query = new QueryExpression();
            query.EntityName = "revfinal_mortgage";
            query.ColumnSet = new ColumnSet(new string[] { "revfinal_name", "revfinal_mortgagenumber", "revfinal_mortgageterm", "revfinal_mortgageamount", "revfinal_totalpayment", "revfinal_requeststatus" });
            query.Criteria.AddCondition("revfinal_contact", ConditionOperator.Equal, id);

            try
            {
                EntityCollection mortgages = service.RetrieveMultiple(query);
                List<object> list = new List<object>();
                foreach (Entity item in mortgages.Entities)
                {
                    decimal total = 0;
                    int status = 273250002;
                    if (item.Attributes.Contains("revfinal_totalpayment"))
                    {
                        Money temp = (Money)item.Attributes["revfinal_totalpayment"];
                        total = temp.Value;
                    }
                    if (item.Attributes.Contains("revfinal_requeststatus"))
                    {
                        OptionSetValue value = (OptionSetValue)item.Attributes["revfinal_requeststatus"];
                        status = value.Value;
                    }
                    var mortgage = new
                    {
                        revfinal_name = item.Attributes["revfinal_name"],
                        revfinal_mortgagenumber = item.Attributes["revfinal_mortgagenumber"],
                        revfinal_mortgageterm = item.Attributes["revfinal_mortgageterm"],
                        revfinal_mortgageamount = item.Attributes["revfinal_mortgageamount"],
                        revfinal_totalpayment = total,
                        revfinal_requeststatus = status
                    };
                    list.Add(mortgage);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        [HttpPost]
        public IHttpActionResult Post([FromBody]MortgageEntity mortgage)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            Entity Mortgage = new Entity("revfinal_mortgage");
            Mortgage.Attributes.Add("revfinal_name", mortgage.MortgageName);
            Mortgage.Attributes.Add("revfinal_contact", new EntityReference("contact",new Guid(mortgage.ContactID)));
            Mortgage.Attributes.Add("revfinal_mortgageterm", new OptionSetValue(mortgage.Option));
            Mortgage.Attributes.Add("revfinal_mortgageamount", new Money(mortgage.MortgageAmount));

            try
            {
                string id = service.Create(Mortgage).ToString();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }
    }

    public class MortgageEntity
    {
        public string MortgageName { get; set; }
        public string ContactID { get; set; }
        public decimal MortgageAmount { get; set; }
        public int Option { get; set; }
    }
}
