using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System.ServiceModel;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Metadata;
using System.Net.Http;
using Newtonsoft.Json;

namespace MortgageConsoleApp
{
    public class Response
    {
        public bool success { get; set; }
        public long timestamp { get; set; }
        [JsonProperty(PropertyName = "base")]
        public string currency { get; set; }
        public string date { get; set; }
        public IDictionary<string, decimal> rates { get; set; }
    }

    public class populateAPR
    {
        public static async Task<Response> GetJson()
        {
            HttpClient httpclient = new HttpClient();
            HttpResponseMessage res = await httpclient.GetAsync("http://data.fixer.io/api/latest?access_key=4e5228e36a1c882b95b9a14a8dc1c2ef");
            var result = res.Content.ReadAsStringAsync().Result;
            Response exchange = JsonConvert.DeserializeObject<Response>(result);
            return exchange;
        }

        static async Task<Decimal> GetAPR()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://team3webapi.azurewebsites.net/api/BaseAPR");
            var content = response.Content.ReadAsStringAsync().Result;
            var apr = JsonConvert.DeserializeObject(content);
            return Convert.ToDecimal(apr);
        }

        //code to autopopulate Mortgage Number field in Mortgage request entity, only needed to be run once for this take affect
        public void autoPopulateMortgageNumber()
        {
            CrmServiceClient client = new CrmServiceClient("Url=https://revaturefinalproj.crm.dynamics.com; Username=ssfletcher@revaturefinalproj.onmicrosoft.com; Password=Revature2018!; authtype=Office365");
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;


            CreateAttributeRequest widgetSerialNumberAttributeRequest = new CreateAttributeRequest
            {
                EntityName = "revfinal_mortgage",
                Attribute = new StringAttributeMetadata
                {
                    //Define the format of the attribute
                    AutoNumberFormat = "{DATETIMEUTC:yyyyMMddhhmm}",
                    LogicalName = "revfinal_mortgagenumber",
                    SchemaName = "revfinal_mortgagenumber",
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    MaxLength = 100, // The MaxLength defined for the string attribute must be greater than the length of the AutoNumberFormat value, that is, it should be able to fit in the generated value.
                    DisplayName = new Label("Mortgage Number", 1033),
                    Description = new Label("Mortgage Number of the Mortgage Application.", 1033)
                }
            };
            service.Execute(widgetSerialNumberAttributeRequest);


        }

        static void Main(string[] args)
        {

            /*
             *This code is run through task manager every day to update the apr for pending mortgage requests
             */

            CrmServiceClient client = new CrmServiceClient("Url=https://revaturefinalproj.crm.dynamics.com; Username=ssfletcher@revaturefinalproj.onmicrosoft.com; Password=Revature2018!; authtype=Office365");
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            //populateAPR hey = new populateAPR();
            //populateAPR



            ////gets new Base Apr
            var apr = GetAPR();

            ////query to get the baseConfig record that will hold 
            QueryExpression query = new QueryExpression();
            query.EntityName = "revfinal_configuration";
            query.ColumnSet = new ColumnSet(new string[] { "revfinal_value" });
            query.Criteria.AddCondition("revfinal_name", ConditionOperator.Equal, "baseApr");
            EntityCollection collection = service.RetrieveMultiple(query);

            Entity config = collection.Entities[0];
            config.Attributes["revfinal_value"] = apr.Result.ToString();
            ////Entity config = new Entity("revfinal_configuration");

            ////updates baseConfig attribute
            service.Update(config);

            Task<Response> response = Task.Run(() => GetJson());
            Response result = response.Result;
            decimal usd = result.rates["USD"];
            decimal cad = result.rates["CAD"];
            decimal exchange = cad / usd;
            Entity currency = new Entity("transactioncurrency", new Guid("9E5D5396-BAB2-E811-A96B-000D3A1CA939"));
            currency.Attributes.Add("exchangerate", exchange);

            service.Update(currency);

           

        }
    }
}
