using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface IAuditLogger
{
    Task<OperationStatusDTO> LogAsync(string action, string entityName, string entityId, string? details = null);
}
