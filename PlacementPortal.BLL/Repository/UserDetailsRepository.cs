using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;

namespace PlacementPortal.BLL.Repository;

public class UserDetailsRepository : GenericRepository<UserDetail>, IUserDetailsRepository
{
    public UserDetailsRepository(PlacementPortalDbContext placementPortalDbContext) : base(placementPortalDbContext)
    {
    }
    
}
