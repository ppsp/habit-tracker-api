using HyperTaskCore.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HyperTaskTools
{
    public class AzureDevopsConnector
    {
        private Uri Uri { get; set; }
        private string PersonalAccessToken { get; set; }
        private string Project { get; set; }

        private VssBasicCredential Credentials { get; set; }

        public AzureDevopsConnector(string uri, string personnalAccessToken, string project)
        {
            this.Uri = new Uri(uri);
            this.PersonalAccessToken = personnalAccessToken;
            this.Project = project;
            this.Credentials = new VssBasicCredential("", this.PersonalAccessToken);
        }

        public async Task<bool> InsertWorkItemAsync(JsonPatchDocument document, eBugReportType type)
        {
            VssConnection connection = new VssConnection(this.Uri, this.Credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var taskType = type == eBugReportType.Bug ? "Bug" : "Task";
            WorkItem result = await workItemTrackingHttpClient.CreateWorkItemAsync(document, this.Project, taskType);
            return true;
        }
    }
}
