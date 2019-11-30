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

            if (!_bodyModelsStorage.Contains(bodyTelemetry.BodyModelSeries, bodyTelemetry.BodyRevision))
            {
                return new HealthReport
                {
                    Error = "Unknown body type"
                };
            }

            var bodyDiagnosticInfo =
                await _bodyModelsStorage.Get(bodyTelemetry.BodyModelSeries, bodyTelemetry.BodyRevision);


            foreach (var (name, refValue) in bodyDiagnosticInfo.ReferenceValues)
                if (bodyTelemetry.HardwareTelemetry.ContainsKey(name))
                {
                    var value = bodyTelemetry.HardwareTelemetry[name];
                    healthReport.CheckResults[name] = value > refValue
                        ? $"ERROR! | {value} > {refValue}"
                        : $"OK | {value} <= {refValue}";
                }
                else
                {
                    healthReport.CheckResults[name] = "unknown";
                }

            return healthReport;
        }
    }
}