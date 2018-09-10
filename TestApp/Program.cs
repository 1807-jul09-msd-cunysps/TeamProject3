using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CrmServiceClient client = new CrmServiceClient("Url=https://cunyraveturetraining.crm.dynamics.com; Username=LinWeiR@revaturefinalproj.onmicrosoft.com; Password=Gaf14492; AuthType=Office365");
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;
            //Task<Response> response = Task.Run(() => GetJson());
            //Response result = response.Result;
            //decimal usd = result.rates["USD"];
            //decimal cad = result.rates["CAD"];

            //decimal exchange = cad / usd;

            //Entity currency = new Entity("transactioncurrency", new Guid("9E5D5396-BAB2-E811-A96B-000D3A1CA939"));
            //currency.Attributes.Add("exchangerate", exchange);
            //service.Update(currency);

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
        <condition attribute='revfinal_contact' operator='eq' uitype='contact' value='{49A0E5B9-88DF-E311-B8E5-6C3BE5A8B200}' />
      </filter>
    </link-entity>
  </entity>
</fetch>";
            //xml = xml.Replace("2bereplaced", "pay");

            try
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = "incident";
                query.ColumnSet = new ColumnSet(new string[] { "title", "ticketnumber", "description", "statuscode" });
                query.Criteria.AddCondition("customerid", ConditionOperator.Equal, "ddb4b385-3fb1-e811-a96b-000d3a1ca939");
                EntityCollection incidents = service.RetrieveMultiple(query);

                Console.ReadLine();

                //EntityCollection articles = service.RetrieveMultiple(new FetchExpression(xml));
                //if (articles.Entities.Any())
                //{
                //    List<object> list = new List<object>();
                //    foreach (Entity item in articles.Entities)
                //    {
                //        var temp = new
                //        {
                //            id = item.Attributes["revfinal_paymentrecordid"],
                //            revfinal_name = item.Attributes["revfinal_name"],
                //            revfinal_mortgagenumber = item.Attributes["mortgage.revfinal_mortgagenumber"],
                //            revfinal_duedate = item.Attributes["revfinal_duedate"],
                //            revfinal_payment = item.Attributes["revfinal_payment"],
                //            revfinal_paymentstatus = item.Attributes["revfinal_paymentstatus"]
                //        };
                //        list.Add(temp);
                //    }
                //    Console.ReadLine();
                //}
            }
            catch (Exception ex)
            {

            }
        }

        public static async Task<Response> GetJson()
        {
            HttpClient httpclient = new HttpClient();
            HttpResponseMessage res = await httpclient.GetAsync("http://data.fixer.io/api/latest?access_key=4e5228e36a1c882b95b9a14a8dc1c2ef");
            var result = res.Content.ReadAsStringAsync().Result;
            Response exchange = JsonConvert.DeserializeObject<Response>(result);
            return exchange;
        }
    }
    public class Response
    {
        public bool success { get; set; }
        public long timestamp { get; set; }
        [JsonProperty(PropertyName = "base")]
        public string currency { get; set; }
        public string date { get; set; }
        public IDictionary<string, decimal> rates { get; set; }
    }
}
