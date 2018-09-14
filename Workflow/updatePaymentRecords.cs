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
using Newtonsoft.Json;

namespace updateMonthlyPaymentWF
{
    public class updatePaymentRecords : CodeActivity
    {
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
                //Query for all active payment records
                QueryExpression query = new QueryExpression();
                query.EntityName = "revfinal_paymentrecord";
                query.ColumnSet = new ColumnSet(new string[] { "revfinal_payment" });
                query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, "Active");
                EntityCollection collection = service.RetrieveMultiple(query);

                //Queries updated APR value
                QueryExpression query2 = new QueryExpression();
                query.EntityName = "revfinal_configuration";
                query.ColumnSet = new ColumnSet(new string[] { "revfinal_value" });
                query.Criteria.AddCondition("revfinal_name", ConditionOperator.Equal, "baseApr");
                EntityCollection collection2 = service.RetrieveMultiple(query2);

                Entity config = collection2.Entities[0];
                Double apr = Convert.ToDouble(config.Attributes["revfinal_value"]);

                //Query margin value
                QueryExpression query3 = new QueryExpression();
                query.EntityName = "revfinal_configuration";
                query.ColumnSet = new ColumnSet(new string[] { "revfinal_value" });
                query.Criteria.AddCondition("revfinal_name", ConditionOperator.Equal, "Margin");
                EntityCollection collection3 = service.RetrieveMultiple(query2);

                Entity config2 = collection3.Entities[0];
                Double margin = Convert.ToDouble(config2.Attributes["revfinal_value"]);


                for (int i = 0; i < collection.TotalRecordCount; i++)
                {
                    //loop through all active payment records and update monthly payment
                    EntityReference mortgage = ((EntityReference)collection.Entities[0].Attributes["revfinal_mortgagerequestid"]);

                    Entity mortgageReq = service.Retrieve(mortgage.LogicalName, mortgage.Id, new ColumnSet(new string[] { "revfinal_contact", "revfinal_mortgageamount", "revfinal_mortgageterm" }));

                    string terms = mortgageReq.Attributes["revfinal_mortgageterm"].ToString();
                    Double amount = Convert.ToDouble(mortgageReq.Attributes["revfinal_mortgageamount"]);

                    EntityReference contactRef = ((EntityReference)mortgageReq.Attributes["revfinal_contact"]);

                    Entity contact = service.Retrieve(contactRef.LogicalName, contactRef.Id, new ColumnSet(new string[] { "revfinal_riskscore", "address1_country", "address1_stateorprovince" }));

                    Double tax;

                    //Query for State tax or Country tax
                    if (contact.Attributes["address1_country"].ToString() == "Canada")
                    {
                        //Query for Canada's tax 
                        QueryExpression query4 = new QueryExpression();
                        query.EntityName = "revfinal_configuration";
                        query.ColumnSet = new ColumnSet(new string[] { "revfinal_value" });
                        query.Criteria.AddCondition("revfinal_name", ConditionOperator.Equal, "Canada");
                        EntityCollection collection4 = service.RetrieveMultiple(query4);

                        Entity config4 = collection4.Entities[0];
                        tax = Convert.ToDouble(config4.Attributes["revfinal_value"]);
                    }
                    else
                    {
                        //Query for Statetax using the Contact's state field
                        QueryExpression query4 = new QueryExpression();
                        query.EntityName = "revfinal_configuration";
                        query.ColumnSet = new ColumnSet(new string[] { "revfinal_value" });
                        query.Criteria.AddCondition("revfinal_name", ConditionOperator.Equal, contact.Attributes["address1_stateorprovince"].ToString());
                        EntityCollection collection4 = service.RetrieveMultiple(query4);

                        Entity config4 = collection4.Entities[0];
                        tax = Convert.ToDouble(config4.Attributes["revfinal_value"]);
                    }
                    Double finalApr = Convert.ToDouble(((apr + margin) / 100) + Math.Log(Convert.ToDouble(contact.Attributes["revfinal_riskscore"]) + tax));

                    Double monthlyPayment = Convert.ToDouble((amount * (finalApr / 12)) / (1 - Math.Pow((1 + (finalApr / 12)), -(12 * Convert.ToDouble(terms)))));

                    collection.Entities[i].Attributes["revfinal_payment"] = monthlyPayment;

                    service.Update(collection.Entities[i]);
                }
            }
        }
    }
}
