using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;

namespace PlacementPortal.BLL.Repository;

public class NotificationMappingRepository : GenericRepository<NotificationUserMapping>, INotificationMappingRepository
{
    public NotificationMappingRepository(PlacementPortalDbContext placementPortalDbContext) : base(placementPortalDbContext)
    {
    }

}
