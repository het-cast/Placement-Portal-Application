using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;

namespace PlacementPortal.BLL.Repository;

public class AuthenticateRepository : GenericRepository<UserAccount>, IAuthenticationRepository
{

    public AuthenticateRepository(PlacementPortalDbContext context) : base(context)
    {
    }

}
