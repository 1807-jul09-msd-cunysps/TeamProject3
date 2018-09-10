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
    public class NoteController : ApiController
    {
        CrmServiceClient client = new CrmServiceClient("Url=https://revaturefinalproj.crm.dynamics.com; Username=systema@revaturefinalproj.onmicrosoft.com; Password=Nut65215; AuthType=Office365");

        [HttpPost]
        public IHttpActionResult Post([FromBody]FileUpload file)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            Entity note = new Entity("annotation");
            note.Attributes.Add("subject", "Identity Documents");
            note.Attributes.Add("objectid", new EntityReference("contact", new Guid(file.ContactID)));
            note.Attributes.Add("filename", file.FileName);
            note.Attributes.Add("documentbody",file.Base64Data);

            try
            {
                service.Create(note);
                return Ok("Success");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }
    }

    public class FileUpload
    {
        public string ContactID { get; set; }
        public string Base64Data { get; set; }
        public string FileName { get; set; }
    }
}
