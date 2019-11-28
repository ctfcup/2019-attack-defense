using System.Threading.Tasks;
using medlink.Helpers;
using medlink.Storage;
using medlink.Storage.Models;
using Microsoft.AspNetCore.Mvc;

namespace medlink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : SessionControllerBase<BodyTelemetry>
    {
        private readonly IBodyTelemetryStorage _bodyTelemetryStorage;
        private readonly ISerializer _serializer;

        public TelemetryController(ISettings settings, ISessionProvider sessionProvider,
            IBodyTelemetryStorage bodyTelemetryStorage, ISerializer serializer) : base(settings,
            sessionProvider)
        {
            _bodyTelemetryStorage = bodyTelemetryStorage;
            _serializer = serializer;
        }

        [HttpGet]
        public async Task<BodyTelemetry> GetTelemetry()
        {
            return await HandleAuthorizedRequest(_bodyTelemetryStorage.Get);
        }

        [HttpPut]
        public async Task TaskPutTelemetry()
        {
            await HandleAuthorizedRequest(async login =>
            {
                var content = await Request.Body.ReadToEndAsync();
                var bodyTelemetry = _serializer.Deserialize<BodyTelemetry>(content);
                await _bodyTelemetryStorage.Add(bodyTelemetry, login);
            });
        }
    }
}