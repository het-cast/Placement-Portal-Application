using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;

namespace PlacementPortal.BLL.Repository;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(PlacementPortalDbContext placementPortalDbContext) : base(placementPortalDbContext)
    {
    }

}
