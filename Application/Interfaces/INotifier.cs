namespace Application.Interfaces
{
    public interface INotifier
    {
        Task NotifyDashboardUpdate(Guid cocheraId);
    }
}
