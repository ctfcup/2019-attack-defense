using System;
using System.IO;
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
        private readonly IVendorTokens _vendors;

        public BodyModelController(ISettings settings, ISessions sessions,
            IBodyModelsStorage bodyModelsStorage, ISerializer serializer, IVendorTokens vendors) : base(
            settings, sessions)
        {
            _bodyModelsStorage = bodyModelsStorage;
            _serializer = serializer;
            _vendors = vendors;
        }

        [HttpPut]
        public async Task AddDiagnosticInfo()
        {
            Request.Query.TryGetValue("vendorToken", out var vendorToken);
            var content = await Request.Body.ReadToEndAsync();
            Console.WriteLine(content);
            var hardwareDiagnosticInfo = _serializer.Deserialize<BodyDiagnosticInfo>(content);
            await HandleAuthorizedRequest(async login =>
            {
                if (!_vendors.TryGet(login, out var vendor) ||
                    !vendor.Token.Equals(vendorToken))
                {
                    Response.StatusCode = 403;
                    return;
                }

                if (_bodyModelsStorage.Contains(hardwareDiagnosticInfo))
                {
                    //TODO: Normal conflict
                    Response.StatusCode = 1111;
                    return;
                }
                
                await _bodyModelsStorage.Add(hardwareDiagnosticInfo,
                    GetPath(hardwareDiagnosticInfo.ModelSeries, hardwareDiagnosticInfo.Revision));
            });
        }
        
        [HttpGet]
        public async Task<BodyDiagnosticInfo> GetDiagnosticInfo()
        {
            var query = Request.Query;
            query.TryGetValue("modelSeries", out var series);
            query.TryGetValue("revision", out var revision);
            query.TryGetValue("vendorToken", out var vendorToken);
            
            return await HandleAuthorizedRequest(async login =>
            {
                if (!_vendors.TryGet(login, out var vendorInfo) ||
                    !vendorInfo.Token.Equals(vendorToken) || 
                    !vendorInfo.ModelSeries.Contains(series))
                {
                    Response.StatusCode = 403;
                    return null;
                }

                return await _bodyModelsStorage.Get(GetPath(series, revision));
            });
        }

        private static string GetPath(string login, StringValues series)
        {
            return Path.Combine(login, series);
        }
    }
}