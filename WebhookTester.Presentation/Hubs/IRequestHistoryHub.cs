using System;
using System.Threading.Tasks;
using WebhookTester.Presentation.Models;

namespace WebhookTester.Presentation.Hubs
{
    public interface IRequestHistoryHub
    {
        Task RequestRemovido(Guid requestId, Guid clientId);
        Task RequestsEsvaziados(Guid clientId);
        Task NovoRequest(Request request);
    }
}