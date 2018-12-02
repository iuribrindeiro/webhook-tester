using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebhookTester.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        public HomeController(IDistributedCache cache, ILogger<HomeController> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("/{id:guid}")]
        public ActionResult Requests(Guid id)
        {
            var cacheData = _cache.Get(id.ToString());
            var requests = new object[]{};
            if (cacheData == null) {
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromHours(2));
                _cache.Set(id.ToString(), Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(requests)), options);
            } else {
                var jsonStr = Encoding.UTF8.GetString(cacheData);
                requests = JsonConvert.DeserializeObject<object[]>(jsonStr);
            }

            return new OkObjectResult(requests);
        }

        [AcceptVerbs("POST", "GET", "PATCH", "PUT", "DELETE")]
        [Route("/teste-request/{id:guid}")]
        public ActionResult<object[]> SaveRequest(Guid id, object requestBody) 
        {
            try {
                var cacheData = _cache.Get(id.ToString());
                if (cacheData == null)
                    return new NotFoundResult();
                
                var jsonStr = Encoding.UTF8.GetString(cacheData);
                var cachedRequests = JsonConvert.DeserializeObject<List<object>>(jsonStr);
                cachedRequests.Add(requestBody);
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromHours(2));
                _cache.Set(id.ToString(), Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(cachedRequests)), options);
            } catch(Exception exception) {
                _logger.LogCritical(2, exception, "Erro ao salvar novo request no cache");
                return new BadRequestObjectResult(new {Message = "Ocorreu um erro ao salvar o request"});
            }
            
            return new OkResult();
        }
    }
}