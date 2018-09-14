using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;


namespace CaseTeamAssignment
{
    public class CaseTeamAssignment : CodeActivity
    {
        [Input("Country")]
        public InArgument<string> Country { get; set; }
        [Output("OwnerTeam")]
        [ReferenceTarget("team")]
        public OutArgument<EntityReference> Team { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            try
            {


                if (Country.Get(executionContext).Equals("US"))
                {
                    QueryExpression query = new QueryExpression();
                    query.EntityName = "team";
                    query.ColumnSet = new ColumnSet(new string[] { "name" });
                    query.Criteria.AddCondition("name", ConditionOperator.Equal, "USA Team");
                    EntityReference us_team = service.RetrieveMultiple(query).Entities.FirstOrDefault().ToEntityReference();
                    Team.Set(executionContext, us_team);
                }
                else
                {
                    QueryExpression query = new QueryExpression();
                    query.EntityName = "team";
                    query.ColumnSet = new ColumnSet(new string[] { "name" });
                    query.Criteria.AddCondition("name", ConditionOperator.Equal, "Canada Team");
                    EntityReference canada_team = service.RetrieveMultiple(query).Entities.FirstOrDefault().ToEntityReference();
                    Team.Set(executionContext, canada_team);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }
    }
}
