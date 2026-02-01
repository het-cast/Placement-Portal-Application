using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;

namespace PlacementPortal.BLL.Repository;

public class StudentDetailRepository : GenericRepository<StudentDetail>, IStudentDetailRepository
{
    public StudentDetailRepository(PlacementPortalDbContext placementPortalDbContext) : base(placementPortalDbContext)
    {
    }

}
