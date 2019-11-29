using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using medlink.Helpers;
using medlink.Storage;
using Microsoft.AspNetCore.Mvc;

namespace medlink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplateController : SessionControllerBase<IEnumerable<string>>
    {
        private readonly IBodyModelsStorage _bodyModelsStorage;

        public TemplateController(ISettings settings, ISessions sessions,
            IBodyModelsStorage bodyModelsStorage) : base(settings,
            sessions)
        {
            _bodyModelsStorage = bodyModelsStorage;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetTelemetry()
        {
            return await HandleAuthorizedRequest(async loging =>
            {
                var series = Request.Query["modelSeries"];
                var revision = Request.Query["revision"];
                var model = await _bodyModelsStorage.Get(GetPath(series, revision));
                return model.ReferenceValues.Keys;
            });
        }
        
        private static string GetPath(string infoModelSeries, string infoRevision)
        {
            return Path.Combine(infoModelSeries, infoRevision);
        }
    }
}