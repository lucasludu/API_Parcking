using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi
{
    public class SignalRNotifier : INotifier
    {
        private readonly IHubContext<ParkingHub> _hubContext;

        public SignalRNotifier(IHubContext<ParkingHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyDashboardUpdate(Guid cocheraId)
        {
            // Enviamos un mensaje llamado "UpdateDashboard" a todos los clientes
            // Pasamos el cocheraId para que el front sepa si debe recargar
            await _hubContext.Clients.All.SendAsync("UpdateDashboard", cocheraId);
        }
    }
}
