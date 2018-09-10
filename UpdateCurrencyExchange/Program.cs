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

namespace UpdateCurrencyExchange
{
    class Program
    {
        static async void Main(string[] args)
        {
            CrmServiceClient client = new CrmServiceClient("Url=https://cunyraveturetraining.crm.dynamics.com; Username=LinWeiR@revaturefinalproj.onmicrosoft.com; Password=Gaf14492; AuthType=Office365");
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;

            HttpClient httpclient = new HttpClient();
            HttpResponseMessage res = await httpclient.GetAsync("http://data.fixer.io/api/latest?access_key=4e5228e36a1c882b95b9a14a8dc1c2ef");
            var result = res.Content.ReadAsStringAsync().Result;
            Response exchange = JsonConvert.DeserializeObject<Response>(result);

        }
    }

    class Response
    {
        public bool success { get; set; }
        public long timestamp { get; set; }
        public string MyProperty { get; set; }
        [JsonProperty(PropertyName = "base")]
        public string currency { get; set; }
        public string date { get; set; }
        public IDictionary<string,decimal> rates { get; set; }
    }
}
