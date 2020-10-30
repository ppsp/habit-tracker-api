using HyperTaskCore.Models;
using HyperTaskServices.Models.DTO;
using HyperTaskTools;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Threading.Tasks;

//
// SOURCE : Mostly from https://docs.microsoft.com/en-us/azure/devops/integrate/quickstarts/create-bug-quickstart?view=azure-devops
//

namespace HyperTaskServices.Services
{
    public class BugReportService
    {
        private AzureDevopsConnector Connector { get; set; }

        public BugReportService(AzureDevopsConnector connector)
        {
            this.Connector = connector;
        }

        public async Task<bool> CreateAzureDevopsWorkItemAsync(DTOBugReport report)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            //add fields and their values to your patch document
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = report.Title
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                    Value = $"UserId = {report.UserId} , Description = {report.Description}",
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.Priority",
                    Value = "2"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.Severity",
                    Value = "2 - High"
                }
            );

            if (report.BugReportType == eBugReportType.Bug)
            {
                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.WorkItemType",
                        Value = "Bug"
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Tags",
                        Value = "Bug"
                    }
                );
            }

            if (report.BugReportType == eBugReportType.Suggestion)
            {
                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.WorkItemType",
                        Value = "Task"
                    }
                );

                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.Tags",
                        Value = "Feature"
                    }
                );
            }

            try
            {
                bool result = await this.Connector.InsertWorkItemAsync(patchDocument, report.BugReportType);

                Logger.Info("Bug Successfully Created: Bug #{0}" + result);

                return true;
            }
            catch (AggregateException ex)
            {
                Logger.Error("Bug Successfully Created: Bug #{0}", ex);

                return false;
            }
        }
    }
}
