using System;

namespace WebhookTester.Presentation.Exceptions
{
    public class RequestNaoExisteException : Exception
    {
        public Guid RequestId { get; }
        public RequestNaoExisteException(Guid requestId) : base($"Request: {requestId} nao existe")
        {
            RequestId = requestId;
        }
    }
}