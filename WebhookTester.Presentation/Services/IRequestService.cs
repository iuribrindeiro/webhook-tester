using System;
using WebhookTester.Presentation.Models;

namespace WebhookTester.Presentation.Services
{
    public interface IRequestService
    {
        Request[] GetRequestsByClientId(Guid clientId);
        void Save(Request request);
    }
}