using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using medlink.Helpers;
using medlink.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace medlink.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BodyModelController : SessionControllerBase<BodyDiagnosticInfo>
    {
        private readonly IBodyModelsStorage _bodyModelsStorage;
        private readonly ISerializer _serializer;
        private readonly IVendorTokenSource _vendorTokenSource;


        [HttpPut]
        public async Task AddDiagnosticInfo()
        {
            Request.Query.TryGetValue("vendorToken", out var token);
            var content = await Request.Body.ReadToEndAsync();
            Console.WriteLine(content);
            var hardwareDiagnosticInfo = _serializer.Deserialize<BodyDiagnosticInfo>(content);
            await HandleAuthorizedRequest(async login =>
            {
                if (!_vendorTokenSource.CheckToken(login, token))
                {
                    Response.StatusCode = 403;
                }

                await _bodyModelsStorage.Add(hardwareDiagnosticInfo,
                    GetPath(login, hardwareDiagnosticInfo.ModelSeries));
            });
        }


        [HttpGet]
        public async Task<BodyDiagnosticInfo> GetDiagnosticInfo()
        {
            var query = Request.Query;
            query.TryGetValue("modelSeries", out var series);
            query.TryGetValue("vendorToken", out var vendorToken);


            return await HandleAuthorizedRequest(async login =>
            {
                if (!_vendorTokenSource.CheckToken(login, vendorToken))
                {
                    Response.StatusCode = 403;
                    return null;
                }
                return await _bodyModelsStorage.Get(GetPath(login, series));
            });
        }

        private static string GetPath(string login, StringValues series)
        {
            return Path.Combine(login, series);
        }

        public BodyModelController(ISettings settings, ISessionProvider sessionProvider, IBodyModelsStorage bodyModelsStorage, ISerializer serializer, IVendorTokenSource vendorTokenSource) : base(settings, sessionProvider)
        {
            _bodyModelsStorage = bodyModelsStorage;
            _serializer = serializer;
            _vendorTokenSource = vendorTokenSource;
        }
    }
}