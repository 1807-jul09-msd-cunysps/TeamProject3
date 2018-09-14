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


namespace PaymentRecordWorkflow
{
    public class PaymentRecordWF : CodeActivity
    {
        [Input("Mortgage Number")]
        public InArgument<string> mortgageNumber { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {

            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            
            QueryExpression query5 = new QueryExpression();
            query5.EntityName = "revfinal_mortgage";
            query5.ColumnSet = new ColumnSet(new string[] { "revfinal_name", "revfinal_mortgageid", "revfinal_mortgageamount", "revfinal_mortgagenumber", "revfinal_mortgageterm", "revfinal_riskscore", "revfinal_contact", "revfinal_finalapr" });
            query5.Criteria.AddCondition("revfinal_mortgagenumber", ConditionOperator.Equal, mortgageNumber.Get(executionContext).ToString());
            EntityCollection mortgageCollection = service.RetrieveMultiple(query5);

            Entity mortgage = mortgageCollection.Entities.FirstOrDefault();

            Double terms = Convert.ToDouble(mortgage.FormattedValues["revfinal_mortgageterm"]);
            var amount = Convert.ToDouble(((Money)mortgage.Attributes["revfinal_mortgageamount"]).Value);
            var risk = Convert.ToDouble(mortgage["revfinal_riskscore"]);

            EntityReference entity = (EntityReference)mortgage.Attributes["revfinal_contact"];
            //Console.WriteLine(entity.Id);
            Console.WriteLine($"Terms: {terms}");
            Entity contact = service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(new string[] { "address1_country", "address1_stateorprovince" }));

            QueryExpression query = new QueryExpression();
            query.EntityName = "revfinal_configuration";
            query.ColumnSet = new ColumnSet(new string[] { "revfinal_value" });
            query.Criteria.AddCondition("revfinal_name", ConditionOperator.Equal, "baseApr");
            EntityCollection collection = service.RetrieveMultiple(query);

            Entity config = collection.Entities.FirstOrDefault();
            Double apr = Convert.ToDouble(config.Attributes["revfinal_value"]);

            //Console.WriteLine($"apr:{apr}");
            //Console.WriteLine(contact["address1_country"]);
            //Console.WriteLine(contact["address1_stateorprovince"]);
            //Retrieved Contact in order to get risk score, state, and country
            //Entity contact = service.Retrieve(contactRef.LogicalName, contactRef.Id, new ColumnSet(new string[] { "revfinal_riskscore", "address1_country", "address1_stateorprovince" }));

            //Query for margin
            QueryExpression query2 = new QueryExpression();
            query2.EntityName = "revfinal_configuration";
            query2.ColumnSet = new ColumnSet(new string[] { "revfinal_value" });
            query2.Criteria.AddCondition("revfinal_name", ConditionOperator.Equal, "Margin");
            EntityCollection collection2 = service.RetrieveMultiple(query2);

            Entity config2 = collection2.Entities.FirstOrDefault();
            Double margin = Convert.ToDouble(config2.Attributes["revfinal_value"]);


            Double tax;
            //EntityReference currencyID;

            //Query for State tax or Country tax
            if (contact.Attributes["address1_country"].ToString() == "Canada")
            {
                //Query for Canada's tax 
                QueryExpression query3 = new QueryExpression();
                query3.EntityName = "revfinal_configuration";
                query3.ColumnSet = new ColumnSet(new string[] { "revfinal_value" });
                query3.Criteria.AddCondition("revfinal_name", ConditionOperator.Equal, "Canada");
                EntityCollection collection3 = service.RetrieveMultiple(query3);

                Entity config3 = collection3.Entities[0];
                tax = Convert.ToDouble(config3.Attributes["revfinal_value"]);

                //Query for Canada's currency
                QueryExpression query6 = new QueryExpression();
                query6.EntityName = "transactioncurrency";
                query6.ColumnSet = new ColumnSet(new string[] { "transactioncurrencyid" });
                query6.Criteria.AddCondition("currencyname", ConditionOperator.Equal, "Canadian Dollar");
                EntityCollection canadaCollection = service.RetrieveMultiple(query6);

                Entity config4 = canadaCollection.Entities[0];
                //mortgage["transactioncurrencyid"] = config4["transactioncurrencyid"];


                //currencyID = (EntityReference)config4["transactioncurrencyid"];

            }
            else
            {
                //Query for Statetax using the Contact's state field
                QueryExpression query4 = new QueryExpression();
                query4.EntityName = "revfinal_configuration";
                query4.ColumnSet = new ColumnSet(new string[] { "revfinal_value" });
                query4.Criteria.AddCondition("revfinal_name", ConditionOperator.Equal, contact.Attributes["address1_stateorprovince"].ToString());
                EntityCollection collection4 = service.RetrieveMultiple(query4);

                Entity config4 = collection4.Entities[0];
                tax = Convert.ToDouble(config4.Attributes["revfinal_value"]);

                //Query for US's currency
                QueryExpression query6 = new QueryExpression();
                query6.EntityName = "transactioncurrency";
                query6.ColumnSet = new ColumnSet(new string[] { "transactioncurrencyid" });
                query6.Criteria.AddCondition("currencyname", ConditionOperator.Equal, "US Dollar");
                EntityCollection usCollection = service.RetrieveMultiple(query6);

                Entity config5 = usCollection.Entities[0];
                //mortgage["transactioncurrencyid"] = config5["transactioncurrencyid"];


                //currencyID = (EntityReference)config5["transactioncurrencyid"];
            }
            //Console.WriteLine($"Margin: {margin}");
            //Console.WriteLine(tax);
            Double aprMargin = (apr + margin);
            //Console.WriteLine($"AppMargin: {aprMargin}");
            Double n = terms * 12;

            //Console.WriteLine(n);
            Double finalApr = (aprMargin + Math.Log(risk) + (tax * 100)) / 100;
            Double bottom = (1 - Math.Pow((1 + (finalApr / 12)), (-n)));
            //Console.WriteLine($"Bottom half: {bottom}");
            Double monthlyPayment = (amount * (finalApr / 12)) / (1 - Math.Pow((1 + (finalApr / 12)), (-n)));
            mortgage["revfinal_finalapr"] = finalApr.ToString();
            service.Update(mortgage);
            //Console.WriteLine($"finalApr: {finalApr}");
            //Console.WriteLine($"Monthly Payment: {monthlyPayment}");
            //Console.WriteLine(((Money)mortgage["revfinal_mortgageamount"]).Value.ToString());
            //Console.WriteLine((mortgage.FormattedValues["revfinal_mortgageterm"]));
            //Console.WriteLine(mortgage["revfinal_riskscore"].ToString());
            //Console.WriteLine(terms);
            //Console.WriteLine(amount);
            //Console.WriteLine(mortgage["revfinal_mortgageid"]);
            Guid id = (Guid)mortgage["revfinal_mortgageid"];
               
            Entity paymentRec = new Entity("revfinal_paymentrecord");
            paymentRec["revfinal_name"] = mortgage.Attributes["revfinal_name"].ToString() + DateTime.Today.ToString();
            paymentRec["revfinal_payment"] = new Money(Convert.ToDecimal(monthlyPayment));
            paymentRec["revfinal_mortgagerequestid"] = new EntityReference(mortgage.LogicalName, id);
            paymentRec["revfinal_duedate"] = DateTime.Today.AddDays(30);
            //paymentRec["revfinal_duedate"] = DateTime.Today.AddDays(30);
            Guid paymentID = service.Create(paymentRec);
            Console.Read();




            
        }
    }
}
