using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using System.Net.Http;



namespace CustomWorkflow
{
    public class populateRiskWorkflow : CodeActivity
    {
        [Input("SSN")]
        public InArgument<string> SSN { get; set; }

        [Output("Risk")]
        public OutArgument<string> RiskScore { get; set; }


        static async Task<Decimal> Getrisk(string ssn)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://team3webapi.azurewebsites.net/api/Credit/" + ssn);
            var content = response.Content.ReadAsStringAsync().Result;
            return Convert.ToDecimal(content);

        }

        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            string ssn = SSN.Get(executionContext);

            var riskScore = Getrisk(ssn).Result;
            string risk = riskScore.ToString();

            RiskScore.Set(executionContext, risk);


        }
    }
}
