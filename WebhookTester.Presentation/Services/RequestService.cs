using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebhookTester.Presentation.Exceptions;
using WebhookTester.Presentation.Models;

namespace WebhookTester.Presentation.Services
{
    public class RequestService : IRequestService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RequestService> _logger;

        public RequestService(IDistributedCache cache, ILogger<RequestService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        private void CreateNewRequestStorage(Guid clientId)
        {
            var options = new DistributedCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromHours(2));
            _cache.Set(clientId.ToString(), Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Request[]{})), options);
        }
        
        public Request[] GetRequestsByClientId(Guid clientId)
        {
            try
            {
                var cacheData = _cache.Get(clientId.ToString());
                var requests = new Request[]{};
                if (cacheData == null) {
                    CreateNewRequestStorage(clientId);
                } else {
                    var jsonStr = Encoding.UTF8.GetString(cacheData);
                    requests = JsonConvert.DeserializeObject<Request[]>(jsonStr);
                }

                return requests;
            }
            catch (Exception exception)
            {
                _logger.LogCritical(2, exception, "Erro ao buscar requests anteriores");
                throw;
            }
        }

        public void Save(Request request)
        {
            try
            {
                var cacheData = _cache.Get(request.ClientId.ToString());
                if (cacheData == null)
                    throw new ClientIdNaoExisteException(request.ClientId);

                var jsonStr = Encoding.UTF8.GetString(cacheData);
                var cachedRequests = JsonConvert.DeserializeObject<List<Request>>(jsonStr);

                cachedRequests.Add(request);
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromHours(2));
                _cache.Set(request.ClientId.ToString(), Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(cachedRequests)),
                    options);
            }
            catch (ClientIdNaoExisteException)
            {
                throw;
            }
            catch(Exception exception) {
                _logger.LogCritical(2, exception, "Erro ao salvar novo request no cache");
                throw;
            }
        }

        public void Delete(Guid requestId, Guid clientId)
        {
            try
            {
                var cacheData = _cache.Get(clientId.ToString());
                var requests = new List<Request>();
                if (cacheData == null)
                    throw new RequestNaoExisteException(requestId);
            
                var jsonStr = Encoding.UTF8.GetString(cacheData);
                requests = JsonConvert.DeserializeObject<List<Request>>(jsonStr);
            
                if (!requests.Any(r => r.Id == requestId))
                    throw new RequestNaoExisteException(requestId);
            
                requests.Remove(requests.Find(r => r.Id == requestId));
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromHours(2));
                _cache.Set(clientId.ToString(), Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(requests)),
                    options);
            }
            catch (RequestNaoExisteException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogCritical(2, exception, "Erro ao salvar novo request no cache");
                throw;
            }
        }

        public void DeleteAll(Guid clientId)
        {
            try
            {
                CreateNewRequestStorage(clientId);
            }
            catch (Exception exception)
            {
                _logger.LogCritical(2, exception, $"Erro ao remover todos os requests do client id: {clientId}");
                throw;
            }
        }
    }
}