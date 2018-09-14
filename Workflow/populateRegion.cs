using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;

namespace populationRegion
{
    public class populateRegion : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            ITracingService tracingService =
               (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory =
                   (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {

                Entity mortgage = (Entity)context.InputParameters["Target"];

                try
                {
                    QueryExpression query = new QueryExpression();
                    query.EntityName = "revfinal_mortgage";
                    query.ColumnSet = new ColumnSet(new string[]
                    {
                        "revfinal_region",
                        "revfinal_contact"
                    });

                    EntityCollection mortgageCollection = service.RetrieveMultiple(query);

                    foreach(Entity item in mortgageCollection.Entities)
                    {
                        if (!item.Attributes.Contains("revfinal_region"))
                        {
                            EntityReference contactRef = (EntityReference)item.Attributes["revfinal_contact"];

                            Entity contact = service.Retrieve(contactRef.LogicalName, contactRef.Id, new ColumnSet(new string[] { "address1_country" }));

                            item.FormattedValues["revfinal_region"] = contact["address1_country"].ToString();

                            service.Update(item);
                        }
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in MyPlug-in.", ex);
                }
                catch (Exception ex)
                {
                    tracingService.Trace("MyPlugin: {0}", ex.ToString());
                    throw;
                }

            }
        }
    }
}
