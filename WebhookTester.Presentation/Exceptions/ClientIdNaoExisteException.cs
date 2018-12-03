using System;

namespace WebhookTester.Presentation.Exceptions
{
    public class ClientIdNaoExisteException : Exception
    {
        public Guid ClientId { get; }
        
        public ClientIdNaoExisteException(Guid clientId) : base($"ClientId: {clientId} nao existe ou ja expirado")
        {
            ClientId = clientId;
        }
    }
}