
using Microsoft.EntityFrameworkCore;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Repository;

public class CompanyJobListingRepository : GenericRepository<CompanyJobListing>, ICompanyJobListingRepository
{

    public CompanyJobListingRepository(PlacementPortalDbContext placementPortalDbContext) : base(placementPortalDbContext)
    {
    }

}
