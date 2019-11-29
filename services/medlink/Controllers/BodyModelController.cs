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
        private readonly IVendorInfos _vendors;

        public BodyModelController(ISettings settings, ISessions sessions,
            IBodyModelsStorage bodyModelsStorage, ISerializer serializer, IVendorInfos vendors) : base(
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
            var info = _serializer.Deserialize<BodyDiagnosticInfo>(content);
            var path = GetPath(info.ModelSeries, info.Revision);

            await HandleAuthorizedRequest(async login =>
            {
                if (!_vendors.TryGet(login, out var vendor) ||
                    !vendor.Token.Equals(vendorToken))
                {
                    Response.StatusCode = 403;
                    return;
                }

                if (_bodyModelsStorage.Contains(path))
                {
                    Response.StatusCode = 409;
                    return;
                }

                vendor.ModelSeries.Add(info.ModelSeries);
                await _bodyModelsStorage.Add(info,path);
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

        private static string GetPath(string infoModelSeries, string infoRevision)
        {
            return Path.Combine(infoModelSeries, infoRevision);
        }
    }
}