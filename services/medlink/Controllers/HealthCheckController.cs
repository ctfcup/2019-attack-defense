using System.Threading.Tasks;
using medlink.HealthChecks;
using medlink.Helpers;
using medlink.Storage;
using medlink.Storage.Models;
using Microsoft.AspNetCore.Mvc;

namespace medlink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : SessionControllerBase<HealthReport>
    {
        private readonly IHealthChecker _healthChecker;
        private readonly IBodyTelemetryStorage _telemetryStorage;

        public HealthCheckController(ISettings settings, IUsers users, ISessions sessions,
            IBodyTelemetryStorage telemetryStorage, IHealthChecker healthChecker) : base(settings,
            sessions)
        {
            _telemetryStorage = telemetryStorage;
            _healthChecker = healthChecker;
        }

        [HttpGet]
        public async Task<HealthReport> GetHealthCheckReports()
        {
            return await HandleAuthorizedRequest(CheckHealth);
        }

        private async Task<HealthReport> CheckHealth(string login)
        {
            if (!_telemetryStorage.Exist(login))
            {
                Response.StatusCode = 404;
                return new HealthReport();
            }

            var bodyTelemetry = await _telemetryStorage.Get(login);
            var healthReport = await _healthChecker.Check(bodyTelemetry);
            return healthReport;
        }
    }
}