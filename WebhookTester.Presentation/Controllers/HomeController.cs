using System;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebhookTester.Presentation.Exceptions;
using WebhookTester.Presentation.Hubs;
using WebhookTester.Presentation.Services;

namespace WebhookTester.Presentation.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IHubContext<RequestHistoryHub> _requestHistoryHub;

        public HomeController(IRequestService requestService, IHubContext<RequestHistoryHub> requestHistoryHub)
        {
            _requestService = requestService;
            _requestHistoryHub = requestHistoryHub;
        }

        [HttpGet("{clientId:guid}")]
        public ActionResult Requests(Guid clientId)
        {
            try
            {
                return new OkObjectResult(_requestService.GetRequestsByClientId(clientId));
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(new {Message = "Ocorreu um erro ao buscar os requests anteriores"});
            }
        }

        [Route("teste-request/{clientId:guid}")]
        public ActionResult SaveRequest(Guid clientId, [FromBody]dynamic requestBody = null)
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

                var request = Models.Request.Create(requestData, HttpContext.Request.Method, clientId);
                _requestService.Save(request);
                _requestHistoryHub.Clients.All
                    .SendAsync("RequestRecebidoEvent", new {request});
            }
            catch (ClientIdNaoExisteException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                Request.Body
                return new BadRequestObjectResult(new {Message = "Ocorreu um erro ao salvar o request"});
            }

            return new OkResult();
        }

        [HttpDelete("remover/{id:guid}/{clientId:guid}")]
        public ActionResult Delete(Guid id, Guid clientId)
        {
            try
            {
                _requestService.Delete(id, clientId);
                _requestHistoryHub.Clients.All
                    .SendAsync("RequestRemovidoEvent", new {requestId = id, clientId});
                return new OkResult();
            }
            catch (RequestNaoExisteException exception)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(new {Message = "Ocorreu um erro ao remover o request"});
            }
        }

        [HttpDelete("remover-todos/{clientId:guid}")]
        public ActionResult DeleteAll(Guid clientId)
        {
            try
            {
                _requestService.DeleteAll(clientId);
                _requestHistoryHub.Clients.All.SendAsync("RequestsEsvaziadosEvent", new {clientId});
                return new OkResult();
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(new {Message = "Ocorreu um erro ao limpar os requests"});
            }
        }
    }
}