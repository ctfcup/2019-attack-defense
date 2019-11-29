using System.Collections.Generic;
using System.Threading.Tasks;
using medlink.Storage;
using Microsoft.AspNetCore.Mvc;

namespace medlink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BodyModelsController : ControllerBase
    {
        private readonly IBodyModelsStorage _bodyModelsStorage;

        public BodyModelsController(IBodyModelsStorage bodyModelsStorage)
        {
            _bodyModelsStorage = bodyModelsStorage;
        }

        [HttpGet]
        public async Task<IEnumerable<SeriesIndexRecord>> GetDiagnosticInfo()
        {
            return _bodyModelsStorage.GetAllModels();
        }
    }
}