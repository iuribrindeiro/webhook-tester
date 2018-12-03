using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebhookTester.Presentation.Models;

namespace WebhookTester.Presentation.Hubs
{
    public class RequestHistoryHub : Hub
    {
        public Task NovoRequest(Request request)
        {
            
        }      
    }
}