using Microsoft.EntityFrameworkCore;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;
using Microsoft.Extensions.Logging;
using System;

namespace PlacementPortal.BLL.Repository;

public class JobApplicationRepository : GenericRepository<JobApplication>, IJobApplicationRepository
{

    public JobApplicationRepository(PlacementPortalDbContext context) : base(context)
    {
    }

}
