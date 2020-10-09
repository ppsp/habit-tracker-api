using System;
using System.Collections.Generic;
using System.Text;

namespace HyperTaskTools
{
    public class ApplicationInsightsConnector
    {
        public ApplicationInsightsConnector(string instrumentationKey)
        {
            Environment.SetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", instrumentationKey);
        }
    }
}
