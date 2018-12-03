using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace WebhookTester.Presentation.Models
{
    public class Request
    {
        public string Method { get; }
        public JObject Data { get; }
        public DateTime Date { get; private set; }
        public Guid ClientId { get; }

        [JsonConstructor]
        public Request(JObject data, string method, DateTime date, string clientId)
        {
            Data = data;
            Method = method;
            Date = date;
            ClientId = Guid.Parse(clientId);
        }

        
        public static Request Create(JObject requestData, string httpMethod, Guid clientId)
        {
            var requestViewModel = new Request(requestData, httpMethod, DateTime.Now, clientId.ToString());
            return requestViewModel;
        }
    }
}