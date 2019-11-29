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
    public class BodyModelController : SessionControllerBase<BodyModelInfo>
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
            var info = _serializer.Deserialize<BodyModelInfo>(content);

            await HandleAuthorizedRequest(async login =>
            {
                if (!_vendors.TryGet(login, out var vendor))
                {
                    Response.StatusCode = 403;
                    return;
                }

                var isVendorTokenValid = vendor.Token.Equals(vendorToken);
                var isAnotherVendorsModel = !vendor.ModelSeries.Contains(info.ModelSeries) && 
                                           _bodyModelsStorage.Contains(info.ModelSeries);
                
                if (!isVendorTokenValid || isAnotherVendorsModel)
                {
                    Response.StatusCode = 403;
                    return;
                }

                if (_bodyModelsStorage.Contains(info.ModelSeries, info.Revision))
                {
                    Response.StatusCode = 409;
                    return;
                }

                vendor.ModelSeries.Add(info.ModelSeries);
                await _bodyModelsStorage.Add(info);
            });
        }
        
        [HttpGet]
        public async Task<BodyModelInfo> GetDiagnosticInfo()
        {
            var query = Request.Query;
            if(!query.TryGetValue("modelSeries", out var series) ||
            !query.TryGetValue("revision", out var revision) ||
            !query.TryGetValue("vendorToken", out var vendorToken))
                throw new ArgumentException("Invalid arguments");
            
            return await HandleAuthorizedRequest(async login =>
            {
                var isVendorExist = _vendors.TryGet(login, out var vendor);
                var isVendorTokenValid = vendor.Token.Equals(vendorToken);
                var isVendorsModel = vendor.ModelSeries.Contains(series);
                
                if (!isVendorExist || !isVendorTokenValid || !isVendorsModel)
                {
                    Response.StatusCode = 403;
                    return null;
                }

                return await _bodyModelsStorage.Get(series, revision);
            });
        }
    }
}