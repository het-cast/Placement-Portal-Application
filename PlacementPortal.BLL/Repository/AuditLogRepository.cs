using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;

namespace PlacementPortal.BLL.Repository;

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(PlacementPortalDbContext placementPortalDbContext) : base(placementPortalDbContext)
    {
    }

}
