using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebhookTester.Presentation.Exceptions;
using WebhookTester.Presentation.Models;
using WebhookTester.Presentation.Services;

namespace WebhookTester.Presentation.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public HomeController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet("{id:guid}")]
        public ActionResult Requests(Guid id)
        {
            try
            {
                return new OkObjectResult(_requestService.GetRequestsByClientId(id));
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(new {Message = "Ocorreu um erro ao buscar os requests anteriores"});
            }
        }

        [Route("teste-request/{id:guid}")]
        public ActionResult SaveRequest(Guid id, [FromBody] dynamic requestBody = null)
        {
            try
            {
                JObject requestData = null;
                if (HttpContext.Request.Method != "GET")
                    requestData = JsonConvert.DeserializeObject<JObject>(requestBody.ToString());
                else
                {
                    var dict = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString());
                    requestData = JObject.FromObject(dict.AllKeys.ToDictionary(k => k, k => dict[k]));
                }

                _requestService.Save(Models.Request.Create(requestData, HttpContext.Request.Method, id));
            }
            catch (ClientIdNaoExisteException)
            {
                return new NotFoundResult();    
            }
            catch(Exception exception) {
                return new BadRequestObjectResult(new {Message = "Ocorreu um erro ao salvar o request"});
            }
            
            return new OkResult();
        }
    }
}