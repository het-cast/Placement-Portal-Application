using Microsoft.EntityFrameworkCore;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Repository;

public class CompanyProfileRepository : GenericRepository<CompanyProfile>, ICompanyProfileRepository
{

    public CompanyProfileRepository(PlacementPortalDbContext placementPortalDbContext) : base(placementPortalDbContext)
    {
    }
}
