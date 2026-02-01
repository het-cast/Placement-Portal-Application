using Microsoft.EntityFrameworkCore;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Repository;

public class ResumeRepository : GenericRepository<Resume>, IResumeRepository
{

    public ResumeRepository(PlacementPortalDbContext placementPortalDbContext) : base(placementPortalDbContext)
    {
    }

}
