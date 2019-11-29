using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using medlink.Storage;
using medlink.Storage.Models;

namespace medlink.HealthChecks
{
    public class HealthChecker : IHealthChecker
    {
        private readonly IBodyModelsStorage _bodyModelsStorage;

        public HealthChecker(IBodyModelsStorage bodyModelsStorage)
        {
            _bodyModelsStorage = bodyModelsStorage;
        }

        public async Task<HealthReport> Check(BodyTelemetry bodyTelemetry)
        {
            var healthReport = new HealthReport
            {
                CheckResults = new Dictionary<string, string>()
            };

            var path = PathHelper.BodyModelPath(bodyTelemetry.BodyModelSeries, bodyTelemetry.BodyRevision);
            if (!_bodyModelsStorage.Exist(path))
            {
                return new HealthReport()
                {
                    Error = "Unknown body type"
                };
            }

            var diagnosticInfo = await _bodyModelsStorage.Get(path);


            foreach (var (name, refValue) in diagnosticInfo.ReferenceValues)
                if (bodyTelemetry.HardwareTelemetry.ContainsKey(name))
                {
                    var value = bodyTelemetry.HardwareTelemetry[name];
                    healthReport.CheckResults[name] = value > refValue
                        ? $"{value} >  |Refs {refValue}"
                        : $"{value} <= |Refs {refValue}";
                }
                else
                {
                    healthReport.CheckResults[name] = "unknown";
                }

            return healthReport;
        }
    }
}