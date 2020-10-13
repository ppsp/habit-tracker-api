using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HyperTaskCore.Services
{
    public interface IReportService
    {
        Task<string> GetTasksCsv(string userId);
    }
}
