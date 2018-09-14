using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;

namespace NoDuplicateContacts
{
    public class SSN : IPlugin
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
                Entity contact = (Entity)context.InputParameters["Target"];
                try
                {
                    QueryExpression query = new QueryExpression();
                    query.EntityName = "contact";
                    query.ColumnSet = new ColumnSet(new string[]
                    {
                        "revfinal_ssn"
                    });
                    EntityCollection collection = service.RetrieveMultiple(query);
                    foreach (Entity item in collection.Entities)
                    {
                        if (item.Attributes.Contains("revfinal_ssn"))
                        {
                            if (contact.Attributes["revfinal_ssn"].Equals(item.Attributes["revfinal_ssn"]))
                            {
                                throw new InvalidPluginExecutionException("Duplicate detected");
                            }
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
