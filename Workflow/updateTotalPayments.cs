using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using System.Net.Http;

namespace UpdatesTotalPayments
{

    public class updateTotalPayments : CodeActivity
    {

        [Input("Mortgage Reference")]
        [ReferenceTarget("revfinal_mortgage")]
        public InArgument<EntityReference> MortgageID { get; set; }


        [Output("Total Payment")]
        public OutArgument<Money> TotalPaymentUpdated { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = "revfinal_paymentrecord";
                query.ColumnSet = new ColumnSet(new string[] { "revfinal_payment", "revfinal_paymentstatus" });
                query.Criteria.AddCondition("revfinal_mortgagerequestid", ConditionOperator.Equal, MortgageID.Get<EntityReference>(executionContext).Id);
                EntityCollection paymentCollection = service.RetrieveMultiple(query);

                Decimal totalPayment = 0;
                for(int i = 0; i < paymentCollection.Entities.Count; i++)
                {
                    if(((OptionSetValue)paymentCollection.Entities[i].Attributes["revfinal_paymentstatus"]).Value == 273250001)
                    {
                        
                        totalPayment += ((Money)paymentCollection.Entities[i].Attributes["revfinal_payment"]).Value;
                    }
                    
                    
                }

               TotalPaymentUpdated.Set(executionContext, new Money(totalPayment));

                
            }
        }
    }
}
