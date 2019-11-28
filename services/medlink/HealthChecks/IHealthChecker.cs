using System.Threading.Tasks;
using medlink.Storage.Models;

namespace medlink.HealthChecks
{
    public interface IHealthChecker
    {
        Task<HealthReport> Check(BodyTelemetry bodyTelemetry);
    }
}