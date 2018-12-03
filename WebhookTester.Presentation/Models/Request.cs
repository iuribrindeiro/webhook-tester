using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace WebhookTester.Presentation.Models
{
    public class Request
    {
        public string Method { get; private set; }
        public JObject Data { get; private set; }
        public DateTime Date { get; private set; }
        public Guid ClientId { get; private set; }
        public Guid Id { get; private set; }

        [JsonConstructor]
        public Request(JObject data, string method, DateTime date, string clientId, string id)
        {
            Data = data;
            Method = method;
            Date = date;
            ClientId = Guid.Parse(clientId);
            Id = Guid.Parse(id);
        }

        protected Request()
        {
        }

        
        public static Request Create(JObject requestData, string httpMethod, Guid clientId)
        {
            var request = new Request();
            request.ClientId = clientId;
            request.Method = httpMethod;
            request.Data = requestData;
            request.Date = DateTime.Now;
            request.Id = Guid.NewGuid();
            
            return request;
        }
    }
}