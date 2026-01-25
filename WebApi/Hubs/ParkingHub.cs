using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs
{
    // Heredamos de Hub. Por ahora lo dejamos vacío.
    // Los métodos para "enviar" mensajes los usaremos desde los Handlers.
    public class ParkingHub : Hub
    {
    }
}
