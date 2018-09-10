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
    public class UserController : ApiController
    {
        CrmServiceClient client = new CrmServiceClient("Url=https://revaturefinalproj.crm.dynamics.com; Username=systema@revaturefinalproj.onmicrosoft.com; Password=Nut65215; AuthType=Office365");

        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            Guid contactid = new Guid(id);

            try
            {
                Entity contact = service.Retrieve("contact", contactid, new ColumnSet(new string[] { "firstname","lastname","emailaddress1","telephone1","mobilephone","address1_line1","address1_line2",
                "address1_city","address1_stateorprovince","address1_country","address1_postalcode"
            }));

                var simple_contact = new
                {
                    firstname = contact.Attributes["firstname"],
                    lastname = contact.Attributes["lastname"],
                    emailaddress1 = contact.Attributes["emailaddress1"],
                    telephone1 = contact.Attributes["telephone1"],
                    mobilephone = contact.Attributes["mobilephone"],
                    address1_line1 = contact.Attributes["address1_line1"],
                    address1_line2 = contact.Attributes["address1_line2"],
                    address1_city = contact.Attributes["address1_city"],
                    address1_stateorprovince = contact.Attributes["address1_stateorprovince"],
                    address1_country = contact.Attributes["address1_country"],
                    address1_postalcode = contact.Attributes["address1_postalcode"]
                };
                return Ok(simple_contact);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }


        [HttpPost]
        public IHttpActionResult Post([FromBody]UserEntity user)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            Entity contact = new Entity("contact");
            contact.Attributes.Add("firstname", user.FirstName);
            contact.Attributes.Add("lastname", user.LastName);
            contact.Attributes.Add("emailaddress1", user.Email);
            contact.Attributes.Add("revfinal_ssn", user.SSN);
            contact.Attributes.Add("telephone1", user.BusinessPhone);
            contact.Attributes.Add("mobilephone", user.MobilePhone);
            contact.Attributes.Add("address1_line1", user.AddressLine1);
            contact.Attributes.Add("address1_line2", user.AddressLine2);
            contact.Attributes.Add("address1_city", user.City);
            contact.Attributes.Add("address1_stateorprovince", user.State);
            contact.Attributes.Add("address1_country", user.Country);
            contact.Attributes.Add("address1_postalcode", user.ZipCode);

            try
            {
                string id = service.Create(contact).ToString();
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }
    }

    public class UserEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SSN { get; set; }
        public string BusinessPhone { get; set; }
        public string MobilePhone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}
