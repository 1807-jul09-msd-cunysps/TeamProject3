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
    public class KnowledgeController : ApiController
    {
        CrmServiceClient client = new CrmServiceClient("Url=https://revaturefinalproj.crm.dynamics.com; Username=systema@revaturefinalproj.onmicrosoft.com; Password=Nut65215; AuthType=Office365");

        [HttpPost]
        public IHttpActionResult Post([FromBody]string text)
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            string xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='knowledgearticle'>
    <attribute name='articlepublicnumber' />
    <attribute name='knowledgearticleid' />
    <attribute name='title' />
    <attribute name='content' />
    <attribute name='rating' />
    <order attribute='articlepublicnumber' descending='false' />
    <filter type='and'>
      <condition attribute='statecode' operator='eq' value='3' />
      <filter type='or'>
        <condition attribute='title' operator='like' value='%2bereplaced%' />
        <condition attribute='content' operator='like' value='%2bereplaced%' />
        <condition attribute='description' operator='like' value='%2bereplaced%' />
        <condition attribute='keywords' operator='like' value='%2bereplaced%' />
      </filter>
    </filter>
  </entity>
</fetch>";
            xml = xml.Replace("2bereplaced", text);

            try
            {
                EntityCollection articles = service.RetrieveMultiple(new FetchExpression(xml));
                if (articles.Entities.Any())
                {
                    List<object> list = new List<object>();
                    foreach (Entity item in articles.Entities)
                    {
                        var temp = new
                        {
                            ArticleID = item.Attributes["knowledgearticleid"],
                            ArticleNumber = item.Attributes["articlepublicnumber"],
                            Title = item.Attributes["title"],
                            Rating = item.Attributes["rating"],
                            Content = item.Attributes["content"]
                        };
                        list.Add(temp);
                    }
                    return Ok(list);
                }
                else
                {
                    return Ok("No Record Found");
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            string xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='knowledgearticle'>
    <attribute name='articlepublicnumber' />
    <attribute name='title' />
    <attribute name='statuscode' />
    <attribute name='content' />
    <attribute name='rating' />
    <order attribute='rating' descending='true' />
    <order attribute='title' descending='false' />
    <filter type='and'>
      <condition attribute='statecode' operator='eq' value='3' />
    </filter>
  </entity>
</fetch>";
            try
            {
                EntityCollection articles = service.RetrieveMultiple(new FetchExpression(xml));
                if (articles.Entities.Any())
                {
                    List<object> list = new List<object>();
                    foreach (Entity item in articles.Entities)
                    {
                        var temp = new
                        {
                            ArticleID = item.Attributes["knowledgearticleid"],
                            ArticleNumber = item.Attributes["articlepublicnumber"],
                            Title = item.Attributes["title"],
                            Rating = item.Attributes["rating"],
                            Content = item.Attributes["content"]
                        };
                        list.Add(temp);
                    }
                    return Ok(list);
                }
                else
                {
                    return Ok("No Record Found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
