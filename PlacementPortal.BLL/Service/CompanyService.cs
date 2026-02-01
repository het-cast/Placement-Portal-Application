using System.Linq.Expressions;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;
using LinqKit;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Helper;
using Hangfire;
using PlacementPortal.BLL.Enums;

namespace PlacementPortal.BLL.Service;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ProtectorHelper _protector;

    private readonly IGlobalService _globalService;

    private readonly EmailHelper _emailHelper;

    private readonly IAuditLogger _auditLogger;

    private readonly INotificationService _notificationService;


    public CompanyService(IUnitOfWork unitOfWork, ProtectorHelper protector, IGlobalService globalService, EmailHelper emailHelper, IAuditLogger auditLogger, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _protector = protector;
        _globalService = globalService;
        _emailHelper = emailHelper;
        _auditLogger = auditLogger;
        _notificationService = notificationService;
    }

    public async Task<PaginationDTO<CompanyJobListingDTO>> GetCompanyJobListingsPaginated(int companyId, PaginationRequestDTO paginationRequest, JobFiltersDTO jobFilters)
    {
        Expression<Func<CompanyJobListing, bool>> expression = PredicateBuilder.New<CompanyJobListing>();

        expression = j => j.IsDeleted == false && j.CompanyId == companyId;
        if (!string.IsNullOrEmpty(paginationRequest.SearchFilter))
        {
            string keyword = paginationRequest.SearchFilter.ToLower();
            expression = expression.And(j => j.Company.Name.ToLower().Contains(keyword) ||
                j.Company.Location!.ToLower().Contains(keyword));
        }

        if (!string.IsNullOrEmpty(jobFilters.JobDomain))
            expression = expression.And(j => j.JobDomain == jobFilters.JobDomain);
        if (jobFilters.MinCgpaRequired > 0)
            expression = expression.And(j => j.CompanyVisit != null && j.CompanyVisit.MinCgpaRequired >= jobFilters.MinCgpaRequired);
        if (jobFilters.MaxBacklogsAllowed >= 0)
            expression = expression.And(j => j.CompanyVisit != null && j.CompanyVisit.MaxBacklogsAllowed <= jobFilters.MaxBacklogsAllowed);
        if (jobFilters.MinPackage > 0)
            expression = expression.And(j => j.MinimumSalary >= jobFilters.MinPackage);
        if (jobFilters.MaxPackage > 0)
            expression = expression.And(j => j.MaximumSalary <= jobFilters.MaxPackage);
        if (jobFilters.StudentBacklog >= 0)
        {
            expression = expression.And(j => j.CompanyVisit != null && j.CompanyVisit.MaxBacklogsAllowed >= jobFilters.StudentBacklog);
        }
        if (jobFilters.StudentCGPA >= 0)
        {
            expression = expression.And(j => j.CompanyVisit != null && jobFilters.StudentCGPA >= j.CompanyVisit.MinCgpaRequired);
        }

        Expression<Func<CompanyJobListing, object>> orderBy = j => j.Id;

        if (!string.IsNullOrEmpty(paginationRequest.SortColumn) && !string.IsNullOrEmpty(paginationRequest.SortOrder))
        {
            orderBy = paginationRequest.SortColumn switch
            {
                FilterConst.IdSort => c => c.Id,
                FilterConst.CreatedAtSort => c => c.CreatedAt,
                _ => c => c.Company.CreatedAt,
            };
        }

        bool isAscending = paginationRequest.SortOrder == FilterConst.Ascending;

        Expression<Func<CompanyJobListing, object>> orderThenBy = j => j.Id;

        (int totalCount, List<CompanyJobListingDTO> companyJobListings) = await _unitOfWork.CompanyJobListingRepository.GetListPaginated(
                                                            expression,
                                                            j => new CompanyJobListingDTO
                                                            {
                                                                ApplicationEndDate = j.CompanyVisit != null ? j.CompanyVisit.ApplicationEndDate : default,
                                                                ApplicationStartDate = j.CompanyVisit != null ? j.CompanyVisit.ApplicationStartDate : default,
                                                                CompanyId = j.CompanyId,
                                                                UniqueGeneratedKey = j.GeneratedKey ?? "",
                                                                JobListingId = j.Id,
                                                                JobDomain = j.JobDomain ?? "",
                                                                JobProfie = j.JobProfile ?? "",
                                                                SalaryUnit = j.SalaryUnit,
                                                                MaxBacklogsAllowed = j.CompanyVisit != null ? j.CompanyVisit.MaxBacklogsAllowed : default,
                                                                MinCgpaRequired = j.CompanyVisit != null ? j.CompanyVisit.MinCgpaRequired : default,
                                                                MaximumSalary = j.MaximumSalary,
                                                                MinimumSalary = j.MinimumSalary,
                                                            },
                                                            orderBy,
                                                            orderThenBy,
                                                            isAscending,
                                                            paginationRequest.CurrentPage,
                                                            paginationRequest.PageSize
                                                        );
        int startIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + 1;
        int endIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + companyJobListings.Count;

        return new PaginationDTO<CompanyJobListingDTO>
        {
            Items = companyJobListings,
            TotalPages = (int)Math.Ceiling((double)totalCount / paginationRequest.PageSize),
            TotalRecords = totalCount,
            PageSize = paginationRequest.PageSize,
            CurrentPage = paginationRequest.CurrentPage,
            StartIndex = companyJobListings.Count > 0 ? startIndex : 0,
            EndIndex = companyJobListings.Count > 0 ? endIndex : 0
        };
    }

    public async Task<PaginationDTO<CompanyProfileDTO>> GetCompanyProfilesPaginated(PaginationRequestDTO paginationRequest, JobFiltersDTO jobFilters)
    {
        Expression<Func<CompanyProfile, bool>> expression = PredicateBuilder.New<CompanyProfile>();

        expression = c => c.IsDeleted == false;
        if (!string.IsNullOrEmpty(paginationRequest.SearchFilter))
        {
            string keyword = paginationRequest.SearchFilter.ToLower();
            expression = expression.And(c => c.Name.ToLower().Contains(keyword) ||
                (c.Location != null && c.Location.ToLower().Contains(keyword)));
        }

        if (!string.IsNullOrEmpty(jobFilters.JobDomain))
            expression = expression.And(c => c.CompanyJobListings != null && c.CompanyJobListings.Any(j => j.JobDomain == jobFilters.JobDomain));
        if (jobFilters.MinCgpaRequired > 0)
            expression = expression.And(c => c.CompanyJobListings != null && c.CompanyJobListings.Any(j => j.CompanyVisit != null && j.CompanyVisit.MinCgpaRequired >= jobFilters.MinCgpaRequired));
        if (jobFilters.MaxBacklogsAllowed >= 0)
            expression = expression.And(c => c.CompanyJobListings != null && c.CompanyJobListings.Any(j => j.CompanyVisit != null && j.CompanyVisit.MaxBacklogsAllowed <= jobFilters.MaxBacklogsAllowed));
        if (jobFilters.MinPackage > 0 && jobFilters.MaxPackage > 0)
            expression = expression.And(c => c.CompanyJobListings != null && c.CompanyJobListings.Any(j => j.MinimumSalary >= jobFilters.MinPackage && j.MaximumSalary <= jobFilters.MaxPackage));
        if (jobFilters.StudentBacklog >= 0)
        {
            expression = expression.And(c => c.CompanyJobListings != null && c.CompanyJobListings.Any(j => j.CompanyVisit != null && j.CompanyVisit.MaxBacklogsAllowed >= jobFilters.StudentBacklog));
        }
        if (jobFilters.StudentCGPA >= 0)
        {
            expression = expression.And(c => c.CompanyJobListings != null && c.CompanyJobListings.Any(j => j.CompanyVisit != null && jobFilters.StudentCGPA >= j.CompanyVisit.MinCgpaRequired));
        }

        Expression<Func<CompanyProfile, object>> orderBy = c => c.Id;

        if (!string.IsNullOrEmpty(paginationRequest.SortColumn) && !string.IsNullOrEmpty(paginationRequest.SortOrder))
        {
            orderBy = paginationRequest.SortColumn switch
            {
                FilterConst.IdSort => c => c.Id,
                FilterConst.CreatedAtSort => c => c.CreatedAt,
                _ => c => c.Id,
            };
        }

        bool isAscending = paginationRequest.SortOrder == FilterConst.Ascending;

        Expression<Func<CompanyProfile, object>> orderThenBy = c => c.Id;

        (int totalCount, List<CompanyProfileDTO> companyProfiles) = await _unitOfWork.CompanyProfileRepository.GetListPaginated(
                                                            expression,
                                                            c => new CompanyProfileDTO
                                                            {
                                                                CompanyId = c.Id,
                                                                CompanyWebsite = c.CompanyWebsite,
                                                                Description = c.Description,
                                                                Location = c.Location,
                                                                Name = c.Name
                                                            },
                                                            orderBy,
                                                            orderThenBy,
                                                            isAscending,
                                                            paginationRequest.CurrentPage,
                                                            paginationRequest.PageSize
                                                        );
        int startIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + 1;
        int endIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + companyProfiles.Count;

        return new PaginationDTO<CompanyProfileDTO>
        {
            Items = companyProfiles,
            TotalPages = (int)Math.Ceiling((double)totalCount / paginationRequest.PageSize),
            TotalRecords = totalCount,
            PageSize = paginationRequest.PageSize,
            CurrentPage = paginationRequest.CurrentPage,
            StartIndex = companyProfiles.Count > 0 ? startIndex : 0,
            EndIndex = companyProfiles.Count > 0 ? endIndex : 0
        };
    }

    public async Task<GenericReturnDTO> AddCompanyProfile(CompanyProfileViewModel companyProfile)
    {
        try
        {
            GenericReturnDTO result = new()
            {
                Success = false
            };

            int idOfLoggedInUser = _globalService.IdOfLoggedInUser();

            CompanyProfile? existingCompany = await _unitOfWork.CompanyProfileRepository
                .GetFirstOrDefault(c => c.Name.ToLower().Trim() == companyProfile.Name.ToLower().Trim() && c.IsDeleted == false)!;

            if (existingCompany != null)
            {
                result.Message = Consts.ExistsMessage(Consts.EntityConst.CompanyProfile);
                return result;
            }

            CompanyProfile company = new()
            {
                Name = companyProfile.Name,
                CompanyWebsite = companyProfile.CompanyWebsite,
                Description = companyProfile.Description,
                Location = companyProfile.Location,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                CreatedBy = idOfLoggedInUser,
                ModifiedBy = idOfLoggedInUser
            };

            await _unitOfWork.CompanyProfileRepository.Add(company);
            await _unitOfWork.SaveChangesAsync();
            string protectedId = _protector.ProtectId(company.Id);

            List<UserAccount> students = await _unitOfWork.AuthenticationRepository.GetList(ua => ua.RoleId == (int)RoleEnum.Student);
            foreach (var student in students)
            {
                string subject = $"{companyProfile.Name} added for the Placement Season";
                string body = $"Hello {student.Email}, New Oppurtunity enrolled for you, {companyProfile.Name} added for the Placement Season";
                BackgroundJob.Enqueue(() => _emailHelper.SendEmailSync(student.Email, "subject", "body"));
            }

            result.Success = true;
            result.Message = Consts.AddedMessage(Consts.EntityConst.CompanyProfile);
            result.Data = protectedId;
            result.Id = company.Id;
            await _auditLogger.LogAsync(Consts.OperationsForLog.Added, Consts.EntityConst.CompanyProfile, company.Id.ToString());
            await _notificationService.SaveNotificationsForStudentsEnhancedMessage(idOfLoggedInUser, Consts.GenerateNotificationMessage.Added(Consts.EntityConst.CompanyProfile, companyProfile.Name));
            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<OperationStatusDTO> UpdateCompanyProfile(CompanyProfileViewModel companyProfile, int companyId)
    {

        OperationStatusDTO result = new();
        companyProfile.CompanyId = companyId;
        int idOfLoggedInUser = _globalService.IdOfLoggedInUser();
        CompanyProfile? existingCompany = await _unitOfWork.CompanyProfileRepository
                                                           .GetFirstOrDefault(c => (c.Name.ToLower().Trim() == companyProfile.Name.ToLower().Trim()) && c.Id != companyId
                                                            && c.IsDeleted == false)!;

        if (existingCompany != null)
        {
            result.Message = Consts.ExistsMessage(Consts.EntityConst.CompanyProfile);
            return result;
        }

        CompanyProfile? targetedCompany = await _unitOfWork.CompanyProfileRepository
                                                           .GetFirstOrDefault(c => c.Id == companyId)!;

        if (targetedCompany == null)
        {
            result.Message = Consts.ExistsMessage(Consts.EntityConst.CompanyProfile);
            return result;
        }

        GenericReturnDTO operationResult = new();
        if (targetedCompany != null)
        {
            targetedCompany.CompanyWebsite = companyProfile.CompanyWebsite;
            targetedCompany.Name = companyProfile.Name;
            targetedCompany.Location = companyProfile.Location;
            targetedCompany.Description = companyProfile.Description;
            targetedCompany.ModifiedAt = DateTime.Now;
            targetedCompany.ModifiedBy = idOfLoggedInUser;
            operationResult = _unitOfWork.CompanyProfileRepository.Update(targetedCompany);
            await _unitOfWork.SaveChangesAsync();
        }

        result.Success = operationResult.Success;
        if (result.Success)
        {
            List<UserAccount> students = await _unitOfWork.AuthenticationRepository.GetList(ua => ua.RoleId == (int)RoleEnum.Student);
            foreach (var student in students)
            {
                BackgroundJob.Enqueue(() => _emailHelper.SendEmailSync(student.Email, "subject", "body"));
            }
            result.Message = Consts.SavedMessage(Consts.EntityConst.CompanyProfile);
            await _auditLogger.LogAsync(Consts.OperationsForLog.Added, Consts.EntityConst.CompanyProfile, targetedCompany!.Id.ToString());
            await _notificationService.SaveNotificationsForStudentsEnhancedMessage(idOfLoggedInUser, Consts.GenerateNotificationMessage.Modified(Consts.EntityConst.CompanyProfile, targetedCompany.Name));
            return result;
        }
        result.Message = Consts.UpdatedFailedMessage(Consts.EntityConst.CompanyProfile);
        return result;
    }

    public async Task<OperationStatusDTO> DeleteCompanyProfile(int companyId)
    {
        OperationStatusDTO result = new();
        int idOfLoggedInUser = _globalService.IdOfLoggedInUser();
        CompanyProfile companyProfile = await _unitOfWork.CompanyProfileRepository.GetFirstOrDefault(c => c.Id == companyId)!;

        if (companyProfile == null)
        {
            result.Message = Consts.CompanyConst.CompanyNotFound;
            return result;
        }

        //It will check if the company has its association with the students with In Progress thing as 
        // NoResponse, Hired and Rejected are complete in itself0
        JobApplication isCompanyRelatedToInProgressStatus = await _unitOfWork.JobApplicationRepository
                                                                             .GetFirstOrDefault(j => j.CompanyListing.CompanyId == companyId && (j.ApplyStatus == (int)JobApplicationEnum.InProgress || j.ApplyStatus == (int)JobApplicationEnum.NoResponse) && j.IsDeleted == false)!;

        if (isCompanyRelatedToInProgressStatus != null)
        {
            result.Message = Consts.CompanyConst.CompanyAssociatedTrue;
            return result;
        }

        companyProfile.IsDeleted = true;
        companyProfile.ModifiedBy = idOfLoggedInUser;
        companyProfile.ModifiedAt = DateTime.Now;

        GenericReturnDTO genericResult = _unitOfWork.CompanyProfileRepository.Update(companyProfile);

        await _unitOfWork.SaveChangesAsync();

        if (!genericResult.Success)
        {
            result.Message = Consts.CompanyConst.CompanyNotDeleted;
            return result;
        }
        result.Success = Consts.GeneralConst.StatusSuccess;
        result.Message = Consts.CompanyConst.CompanyDeleted;
        await _auditLogger.LogAsync(Consts.OperationsForLog.Added, Consts.EntityConst.CompanyProfile, companyId.ToString());
        await _notificationService.SaveNotificationsForStudentsEnhancedMessage(idOfLoggedInUser, Consts.GenerateNotificationMessage.Deleted(Consts.EntityConst.CompanyProfile, companyProfile.Name));
        return result;
    }

    public async Task<GenericReturnDTO> UpdateJobListings(JobListingOperationDTO jobListingData)
    {
        GenericReturnDTO result = new();
        try
        {
            int idOfLoggedInUser = _globalService.IdOfLoggedInUser();

            if (jobListingData.JobListingCommon == null || jobListingData.JobListingCommon.CompanyId == null)
            {
                result.Message = Consts.CompanyConst.CompanyNotFound;
                return result;
            }

            int companyId = _protector.UnProtectId(jobListingData.JobListingCommon.CompanyId);

            CompanyProfile companyProfile = await _unitOfWork.CompanyProfileRepository.GetFirstOrDefault(cp => cp.Id == companyId)!;

            if (companyProfile == null)
            {
                result.Message = Consts.CompanyConst.CompanyNotFound;
                return result;
            }


            if (jobListingData.JobListings == null || !jobListingData.JobListings.Any() || companyId <= 0)
            {
                result.Message = Consts.CompanyConst.NoListingsFoundToUpdate;
                return result;
            }

            List<CompanyJobListing> existingListings = await _unitOfWork.CompanyJobListingRepository
                                                                        .GetListInclude(x => x.CompanyId == companyId && x.IsDeleted == false, x => x.CompanyVisit!);

            CompanyVisit exisingCompanyVisit = await _unitOfWork.CompanyVisitRepository.GetFirstOrDefault(cv => cv.CompanyId == companyId)!;

            exisingCompanyVisit ??= new()
            {
                Id = -1
            };

            Dictionary<string, CompanyJobListing> existingMap = existingListings.ToDictionary(x => $"{x.JobProfile?.ToLower()}-{x.JobDomain?.ToLower()}");
            Dictionary<string, JobListingsDTO> incomingMap = jobListingData.JobListings.ToDictionary(x => $"{x.JobProfie!.ToLower()}-{x.JobDomain!.ToLower()}");

            List<CompanyJobListing> toAdd = new();
            List<CompanyJobListing> toUpdate = new();
            List<CompanyJobListing> toDelete = new();

            foreach (var dto in jobListingData.JobListings)
            {
                // var key = $"{dto.JobProfie.ToLower()}-{dto.JobDomain.ToLower()}";
                string key = dto.UniqueGeneratedKey!;

                if (existingMap.ContainsKey(key))
                {
                    CompanyJobListing entity = existingMap[key];
                    entity.CompanyVisit!.ApplicationStartDate = jobListingData.JobListingCommon.ApplicationStartDate;
                    entity.CompanyVisit.ModifiedBy = _globalService.RoleIdOfLoggedInUser();
                    entity.CompanyVisit.ModifiedAt = DateTime.Now;
                    entity.CompanyVisit.ApplicationEndDate = jobListingData.JobListingCommon.ApplicationEndDate;
                    entity.CompanyVisit.MinCgpaRequired = jobListingData.JobListingCommon.MinCgpaRequired;
                    entity.CompanyVisit.MaxBacklogsAllowed = jobListingData.JobListingCommon.MaxBacklogsAllowed;
                    entity.MinimumSalary = dto.MinimumSalary;
                    entity.MaximumSalary = dto.MaximumSalary;
                    entity.SalaryUnit = jobListingData.JobListingCommon.SalaryUnit;
                    entity.ModifiedAt = DateTime.Now;
                    entity.ModifiedBy = idOfLoggedInUser;
                    toUpdate.Add(entity);
                }
                else
                {
                    CompanyJobListing newEntity = new()
                    {
                        CompanyId = companyId,
                        JobDomain = dto.JobDomain,
                        JobProfile = dto.JobProfie,
                        MinimumSalary = dto.MinimumSalary,
                        MaximumSalary = dto.MaximumSalary,
                        SalaryUnit = jobListingData.JobListingCommon.SalaryUnit,
                        IsDeleted = false,
                        CreatedBy = idOfLoggedInUser,
                        ModifiedBy = idOfLoggedInUser,
                        CompanyVisitId = exisingCompanyVisit.Id
                    };
                    toAdd.Add(newEntity);
                }
            }

            string listingAssociatedWithInProgress = string.Empty;
            foreach (string key in existingMap.Keys)
            {
                if (!incomingMap.ContainsKey(key))
                {
                    CompanyJobListing entity = existingMap[key];
                    var jobListingsAssociatedWithInProgressStatus = await _unitOfWork.JobApplicationRepository.GetFirstOrDefaultInclude(j => j.CompanyListingId == entity.Id, j => j.CompanyListing, j => j.CompanyListing.Company);
                    if (jobListingsAssociatedWithInProgressStatus == null)
                    {
                        entity.IsDeleted = true;
                        entity.ModifiedBy = idOfLoggedInUser;
                        entity.ModifiedAt = DateTime.Now;
                        toUpdate.Add(entity);
                    }
                    else
                    {
                        listingAssociatedWithInProgress += jobListingsAssociatedWithInProgressStatus.CompanyListing.JobDomain + " - " + jobListingsAssociatedWithInProgressStatus.CompanyListing.JobProfile + ", ";
                    }

                }
            }

            if (exisingCompanyVisit == null || exisingCompanyVisit.Id == -1)
            {
                CompanyVisit companyVisit = new()
                {
                    ApplicationEndDate = jobListingData.JobListingCommon.ApplicationEndDate,
                    ApplicationStartDate = jobListingData.JobListingCommon.ApplicationStartDate,
                    CompanyId = companyId,
                    MaxBacklogsAllowed = jobListingData.JobListingCommon.MaxBacklogsAllowed,
                    MinCgpaRequired = jobListingData.JobListingCommon.MinCgpaRequired,
                    ModifiedBy = idOfLoggedInUser,
                    CreatedBy = idOfLoggedInUser,
                    ModifiedAt = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                GenericReturnDTO companyVisitResult = await _unitOfWork.CompanyVisitRepository.Add(companyVisit);
                foreach (var jobListing in toAdd)
                {
                    jobListing.CompanyVisit = companyVisit;
                }
            }

            if (toAdd.Any())
            {
                GenericReturnDTO addResult = await _unitOfWork.CompanyJobListingRepository.AddRange(toAdd);
            }

            if (toUpdate.Any())
            {
                GenericReturnDTO updateResult = _unitOfWork.CompanyJobListingRepository.UpdateRange(toUpdate);
            }
            await _unitOfWork.SaveChangesAsync();


            List<UserAccount> students = await _unitOfWork.AuthenticationRepository.GetList(ua => ua.RoleId == (int)RoleEnum.Student);

            foreach (var student in students)
            {
                string emailBody = EmailConsts.GenerateCompanyUpateBody(companyProfile.Name);
                BackgroundJob.Enqueue(() => _emailHelper.SendEmailSync(student.Email, EmailConsts.CompanyUpdate, emailBody));
            }
            if (!String.IsNullOrEmpty(listingAssociatedWithInProgress))
            {
                result.Data = listingAssociatedWithInProgress;
            }
            result.Success = true;
            result.Message = Consts.CompanyConst.JobListingSavedSuccessfully;
            await _notificationService.SaveNotificationsForStudentsEnhancedMessage(idOfLoggedInUser, Consts.GenerateNotificationMessage.Modified(Consts.EntityConst.JobListing, companyProfile.Name));

        }
        catch (Exception)
        {
            result.Message = Consts.CompanyConst.JobListingFailedToSave;
        }
        return result;
    }

    public async Task<CompanyProfileViewModel> GetCompanyDetailById(int companyId)
    {

        if (companyId == 0)
        {
            return new CompanyProfileViewModel();
        }
        CompanyProfile? existingCompanyProfile = await _unitOfWork.CompanyProfileRepository.GetFirstOrDefault(c => c.IsDeleted == false && c.Id == companyId)!;

        if (existingCompanyProfile == null)
        {
            return new CompanyProfileViewModel();
        }

        CompanyProfileViewModel companyData = new()
        {
            CompanyId = existingCompanyProfile.Id,
            CompanyWebsite = existingCompanyProfile.CompanyWebsite,
            Location = existingCompanyProfile.Location,
            Description = existingCompanyProfile.Description,
            Name = existingCompanyProfile.Name
        };

        return companyData;
    }

    public async Task<List<CompanyJobListingDTO>> GetCompanyJobListings(string companyId)
    {
        try
        {
            int companyIdInt = _protector.UnProtectId(companyId);

            List<CompanyJobListing> jobListings = await _unitOfWork.CompanyJobListingRepository.GetListInclude(jl => jl.CompanyId == companyIdInt && jl.IsDeleted == false, jl => jl.CompanyVisit!);

            List<CompanyJobListingDTO> jobListingsDTO = jobListings.OrderBy(j => j.Id).Select(jl => new CompanyJobListingDTO
            {
                ApplicationEndDate = jl.CompanyVisit!.ApplicationEndDate,
                ApplicationStartDate = jl.CompanyVisit.ApplicationStartDate,
                CompanyId = jl.CompanyId,
                JobDomain = jl.JobDomain!,
                JobProfie = jl.JobProfile!,
                MaxBacklogsAllowed = jl.CompanyVisit.MaxBacklogsAllowed,
                MaximumSalary = jl.MaximumSalary,
                MinCgpaRequired = jl.CompanyVisit.MinCgpaRequired,
                MinimumSalary = jl.MinimumSalary,
                SalaryUnit = jl.SalaryUnit,
                UniqueGeneratedKey = jl.GeneratedKey!
            }).ToList();

            return jobListingsDTO;
        }
        catch (Exception)
        {
            return new List<CompanyJobListingDTO>();
        }
    }

    public async Task<List<CompanyProfile>> GetJobsClosingWithin24HoursAsync()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        DateOnly tomorrow = today.AddDays(1);

        List<CompanyVisit> closingVisits = await _unitOfWork.CompanyVisitRepository
            .GetList(v => v.ApplicationEndDate > today && v.ApplicationEndDate <= tomorrow);

        var companyIds = closingVisits
            .Select(v => v.CompanyId)
            .Distinct()
            .ToList();

        if (!companyIds.Any())
            return new List<CompanyProfile>();

        List<CompanyProfile> companyProfiles = await _unitOfWork.CompanyProfileRepository
            .GetList(cp => companyIds.Contains(cp.Id));

        return companyProfiles;
    }

    public async Task<GenericReturnDTO> GetDistinctJobProfiles()
    {
        GenericReturnDTO data = new();
        try
        {
            List<string> jobProfiles = await _unitOfWork.CompanyJobListingRepository
                                                        .GetDistinctValues(
                                                            j => true,
                                                            j => j.JobProfile!,
                                                            j => j.First().JobProfile!);

            return new GenericReturnDTO()
            {
                Data = jobProfiles,
                Success = Consts.GeneralConst.StatusSuccess,
                Message = Consts.GeneralConst.DateFetched
            };
        }
        catch (Exception)
        {
            data.Message = Consts.GeneralConst.ErrorFetchingData;
            return data;
        }
    }


    public string ProtectId(int id)
    {
        string protectedId = _protector.ProtectId(id);

        return protectedId;
    }

    public int UnProtectId(string id)
    {
        int idInt = _protector.UnProtectId(id);

        return idInt;
    }

}
