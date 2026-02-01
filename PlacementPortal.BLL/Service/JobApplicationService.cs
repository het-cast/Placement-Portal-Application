using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;
using PlacementPortal.BLL.Constants;
using LinqKit;

namespace PlacementPortal.BLL.Service;

public class JobApplicationService : IJobApplicationService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IGlobalService _globalService;

    private readonly INotificationService _notificationService;


    public JobApplicationService(IUnitOfWork unitOfWork, IGlobalService globalService, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _globalService = globalService;
        _notificationService = notificationService;
    }

    public async Task<OperationStatusDTO> SubmitJobApplicationAsync(int jobListingId)
    {
        OperationStatusDTO result = new();

        try
        {
            if (jobListingId <= 0)
            {
                result.Success = false;
                result.Message = Consts.CompanyConst.NoJobListingsFound;
                return result;
            }

            TokenDataDTO tokenData = _globalService.GetTokenData();

            int idOfLoggedInUser = _globalService.IdOfLoggedInUser();

            if (!tokenData.IsTokenPresent)
            {
                result.Success = false;
                result.Message = Consts.GeneralConst.SessionExpired;
                return result;
            }

            bool IsStudentRegistrationComplete = await _globalService.IsStudentRegistrationComplete();

            Resume? resume = await _unitOfWork.ResumeRepository.GetFirstOrDefault(r => r.Student.Email == tokenData.Email)!;

            if (!IsStudentRegistrationComplete || resume == null)
            {
                throw new Exception(CustomExceptionConst.StudentRegistrationInComplete);
            }

            CompanyJobListing? jobListing = await _unitOfWork.CompanyJobListingRepository
                                                             .GetFirstOrDefaultInclude(jl => jl.Id == jobListingId, jl => jl.CompanyVisit!);
            if (jobListing == null)
            {
                result.Success = false;
                result.Message = Consts.NotFound(Consts.EntityConst.JobListing);
                return result;
            }

            StudentDetail? student = await _unitOfWork.StudentDetailRepository.GetFirstOrDefault(s => s.Student.Email == tokenData.Email)!;

            if (student == null)
            {
                result.Success = false;
                result.Message = Consts.NotFound(Consts.EntityConst.Student);
                return result;
            }

            if ((jobListing.CompanyVisit != null && student.Cgpa < jobListing.CompanyVisit.MinCgpaRequired) || (jobListing.CompanyVisit != null && student.PendingBacklog > jobListing.CompanyVisit.MaxBacklogsAllowed))
            {
                result.Success = false;
                result.Message = Consts.StudentConst.EligibilityCriteriaNotMet;
                return result;
            }

            JobApplication? existingApplication = await _unitOfWork.JobApplicationRepository
                                                   .GetFirstOrDefault(a => a.StudentId == student.StudentId && a.CompanyListingId == jobListingId)!;
            if (existingApplication != null)
            {
                result.Success = false;
                result.Message = Consts.JobApplicationConst.AlreadyApplied;
                return result;
            }

            JobApplication jobApplication = new()
            {
                StudentId = student.StudentId,
                CompanyListingId = jobListingId,
                AppliedDate = DateTime.Now,
                ApplyStatus = (int)JobApplicationEnum.NoResponse,
                ModifiedDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                ModifiedBy = idOfLoggedInUser,
                CreatedBy = idOfLoggedInUser
            };

            CompanyProfile company = await _unitOfWork.CompanyProfileRepository.GetFirstOrDefault(c => c.Id == jobListing.CompanyId)!;

            GenericReturnDTO addResult = await _unitOfWork.JobApplicationRepository.Add(jobApplication);
            await _unitOfWork.SaveChangesAsync();

            if (addResult.Success)
            {
                result.Success = Consts.GeneralConst.StatusSuccess;
                result.Message = Consts.JobApplicationConst.SubmitSuccess;
                string notificationMessage = Consts.GenerateNotificationMessage.AppliedToJob(tokenData.Email, company.Name ?? "XYZ Company");
                await _notificationService.SaveNotificationAsyncsForAllTPOs(notificationMessage);
                return result;
            }
            result.Success = Consts.GeneralConst.StatusFailed;
            result.Message = Consts.JobApplicationConst.SubmitFailed;
            return result;
        }
        catch (Exception ex)
        {
            if (ex.Message == CustomExceptionConst.StudentRegistrationInComplete)
            {
                return new OperationStatusDTO
                {
                    Success = false,
                    Message = StudentConst.ShouldCompleteRegistration
                };
            }

            return new OperationStatusDTO
            {
                Success = false,
                Message = Consts.GeneralConst.UnexpectedError
            };
        }
    }

    public async Task<PaginationDTO<StudentJobApplicationDTO>> GetPersonalApplicationsPaginatedEnhanced(PaginationRequestDTO paginationRequest)
    {
        Expression<Func<JobApplication, bool>> whereExpression = PredicateBuilder.New<JobApplication>();

        TokenDataDTO tokenData = _globalService.GetTokenData();

        StudentDetail? student = await _unitOfWork.StudentDetailRepository
                .GetFirstOrDefault(sd => sd.Student.Email.ToLower().Trim() == tokenData.Email)!;


        whereExpression = j => j.IsDeleted == false && j.Student.Email == tokenData.Email;

        if (!string.IsNullOrEmpty(paginationRequest.SearchFilter))
        {
            string keyword = paginationRequest.SearchFilter.ToLower().Trim();
            whereExpression = whereExpression.And(j =>
                j.CompanyListing.Company.Name.ToLower().Contains(keyword) ||
                j.CompanyListing.JobProfile!.ToLower().Contains(keyword) ||
                j.CompanyListing.JobDomain!.ToLower().Contains(keyword)
            );
        }

        bool isAscending = paginationRequest.SortOrder.Equals(FilterConst.Ascending);

        Expression<Func<JobApplication, object>> orderBy = j => j.Id;

        if (!string.IsNullOrEmpty(paginationRequest.SortColumn))
        {
            orderBy = paginationRequest.SortColumn switch
            {
                FilterConst.IdSort => j => j.Id,
                FilterConst.CompanyNameSort => j => j.CompanyListing.Company.Name,
                FilterConst.MinPackageSort => j => j.CompanyListing.MinimumSalary ?? j.Id,
                FilterConst.MaxPackageSort => j => j.CompanyListing.MaximumSalary ?? j.Id,
                FilterConst.CreatedAtSort => j => j.CreatedAt,
                _ => j => j.Id
            };
        }

        Expression<Func<JobApplication, object>> orderThenBy = j => j.Id;

        (int totalCount, List<StudentJobApplicationDTO> studentJobs) = await _unitOfWork.JobApplicationRepository.GetListPaginated(
            whereExpression,
            j => new StudentJobApplicationDTO
            {
                AppliedDate = j.CreatedAt,
                ApplicationId = j.Id,
                ApplyStatus = j.ApplyStatus,
                CompanyName = j.CompanyListing.Company.Name,
                JobDomain = j.CompanyListing.JobDomain ?? "",
                JobListingId = j.CompanyListingId,
                JobProfile = j.CompanyListing.JobProfile ?? "",
                MaxPackageOffered = j.CompanyListing.MaximumSalary ?? 0m,
                MinPackageOffered = j.CompanyListing.MinimumSalary ?? 0m,
                StudentId = j.StudentId,
            },
            orderBy,
            orderThenBy,
            isAscending,
            paginationRequest.CurrentPage,
            paginationRequest.PageSize
        );

        int startIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + 1;
        int endIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + studentJobs.Count;

        return new PaginationDTO<StudentJobApplicationDTO>
        {
            CurrentPage = paginationRequest.CurrentPage,
            TotalPages = (int)Math.Ceiling((double)totalCount / paginationRequest.PageSize),
            PageSize = paginationRequest.PageSize,
            StartIndex = studentJobs.Count > 0 ? startIndex : 0,
            EndIndex = studentJobs.Count > 0 ? endIndex : 0,
            Items = studentJobs,
            TotalRecords = totalCount
        };

    }
    public async Task<PaginationDTO<StudentJobApplicationDTO>> GetApplicationsForTpoPaginated(PaginationRequestDTO paginationRequest, int statusFilter)
    {
        try
        {
            Expression<Func<JobApplication, bool>> whereExpression = PredicateBuilder.New<JobApplication>();

            whereExpression = j => j.IsDeleted == false;

            if (!string.IsNullOrEmpty(paginationRequest.SearchFilter))
            {
                string keyword = paginationRequest.SearchFilter.ToLower().Trim();
                whereExpression = whereExpression.And(j =>
                    j.CompanyListing.Company.Name.ToLower().Contains(keyword) ||
                    (j.CompanyListing.JobProfile != null && j.CompanyListing.JobProfile.ToLower().Contains(keyword)) ||
                    (j.CompanyListing.JobDomain != null && j.CompanyListing.JobDomain.ToLower().Contains(keyword)) ||
                    j.Student.Email.ToLower().Contains(keyword) ||
                    j.Student.UserDetails.Any(ud => ud.UserId == j.StudentId &&
                                                    ud.FirstName != null &&
                                                    ud.FirstName.ToLower().Contains(keyword))
                );
            }

            if (statusFilter > 0)
            {
                whereExpression = whereExpression.And(
                    j => j.ApplyStatus == statusFilter
                );
            }

            bool isAscending = paginationRequest.SortOrder.Equals(FilterConst.Ascending);

            Expression<Func<JobApplication, object>> orderBy = j => j.StudentId;

            if (!string.IsNullOrEmpty(paginationRequest.SortColumn))
            {
                orderBy = paginationRequest.SortColumn switch
                {
                    FilterConst.IdSort => j => j.Id,
                    FilterConst.StudentIdSort => j => j.StudentId,
                    FilterConst.CompanyNameSort => j => j.CompanyListing.Company.Name,
                    FilterConst.MinPackageSort => j => j.CompanyListing.MinimumSalary ?? j.Id,
                    FilterConst.MaxPackageSort => j => j.CompanyListing.MaximumSalary ?? j.Id,
                    FilterConst.CreatedAtSort => j => j.CreatedAt,
                    _ => j => j.Id
                };
            }

            Expression<Func<JobApplication, object>> orderThenBy = j => j.Id;

            (int totalCount, List<StudentJobApplicationDTO> applications) = await _unitOfWork.JobApplicationRepository
                                            .GetListPaginated(
                                                whereExpression,
                                                j => new StudentJobApplicationDTO
                                                {
                                                    ApplicationId = j.Id,
                                                    JobProfile = j.CompanyListing.JobProfile!,
                                                    CompanyName = j.CompanyListing.Company.Name,
                                                    StudentId = j.StudentId,
                                                    AppliedDate = j.AppliedDate,
                                                    ModifiedDate = j.ModifiedDate,
                                                    JobListingId = j.CompanyListingId,
                                                    ApplyStatus = j.ApplyStatus,
                                                    MinPackageOffered = j.CompanyListing.MinimumSalary ?? 0,
                                                    MaxPackageOffered = j.CompanyListing.MaximumSalary ?? 0,
                                                    StudentName = j.Student.UserDetails
                                                    .Select(sd => $"{sd.FirstName} {sd.MiddleName} {sd.LastName}".Trim())
                                                    .FirstOrDefault() ?? Consts.GeneralConst.NotAvailable
                                                },
                                                orderBy,
                                                orderThenBy,
                                                isAscending,
                                                paginationRequest.CurrentPage,
                                                paginationRequest.PageSize
                                            );
            return new PaginationDTO<StudentJobApplicationDTO>
            {
                CurrentPage = paginationRequest.CurrentPage,
                TotalPages = (int)Math.Ceiling((double)totalCount / paginationRequest.PageSize),
                PageSize = paginationRequest.PageSize,
                StartIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + 1,
                EndIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + applications.Count,
                Items = applications,
                TotalRecords = totalCount
            };

        }
        catch (Exception)
        {
            return new PaginationDTO<StudentJobApplicationDTO>
            {
                Items = new List<StudentJobApplicationDTO>(),
                TotalPages = 0,
                TotalRecords = 0,
                PageSize = paginationRequest?.PageSize ?? 10,
                CurrentPage = paginationRequest?.CurrentPage ?? 1
            };
        }
    }

    public async Task<JobApplicationDataDTO> GetApplicationDetails(int applicationId)
    {
        try
        {
            if (applicationId <= 0)
            {
                return null;
            }

            JobApplication application = await _unitOfWork.JobApplicationRepository.GetFirstOrDefaultInclude(
                                                            ja => ja.Id == applicationId,
                                                            a => a.CompanyListing,
                                                            a => a.CompanyListing.Company,
                                                            a => a.Student,
                                                            a => a.Student.UserDetails,
                                                            a => a.Student.StudentDetails
                                                        );

            if (application == null)
            {
                return null;
            }

            StudentDetail studentDetails = await _unitOfWork.StudentDetailRepository.GetFirstOrDefaultInclude(
                sd => sd.StudentId == application.StudentId,
                sd => sd.BranchNavigation
            );

            Resume? resume = await _unitOfWork.ResumeRepository.GetFirstOrDefault(r => r.StudentId == application.StudentId)!;

            JobApplicationDataDTO applicationData = new JobApplicationDataDTO
            {
                StudentId = application.StudentId,
                AppliedDate = application.AppliedDate,
                ModifiedDate = application.ModifiedDate,
                JobListingId = application.CompanyListingId,
                ApplyStatus = application.ApplyStatus,
                MinPackageOffered = application.CompanyListing.MinimumSalary ?? 0,
                MaxPackageOffered = application.CompanyListing.MaximumSalary ?? 0,
                JobProfile = application.CompanyListing.JobProfile ?? Consts.GeneralConst.NotAvailable,
                JobDomain = application.CompanyListing.JobDomain ?? Consts.GeneralConst.NotAvailable,
                CompanyName = application.CompanyListing.Company?.Name ?? Consts.GeneralConst.NotAvailable,
                StudentName = application.Student.UserDetails
                            .Select(ud => $"{ud.FirstName} {ud.MiddleName} {ud.LastName}".Trim())
                            .FirstOrDefault() ?? Consts.GeneralConst.NotAvailable,
                ResumePath = resume?.ResumeFileUrl,
                ResumeFileName = resume?.ResumeName,
                StudentEmail = application.Student?.Email ?? Consts.GeneralConst.NotAvailable,
                StudentDepartment = studentDetails?.BranchNavigation?.Name ?? Consts.GeneralConst.NotAvailable
            };

            return applicationData;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<OperationStatusDTO> UpdateApplicationStatus(int applicationId, int applyStatus)
    {
        OperationStatusDTO result = new();

        try
        {

            TokenDataDTO tokenData = _globalService.GetTokenData();

            UserAccount userAccount = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(ua => ua.Email == tokenData.Email)!;

            JobApplication jobApplication = await _unitOfWork.JobApplicationRepository.GetFirstOrDefault(j => j.Id == applicationId)!;

            if (jobApplication == null)
            {
                result.Message = Consts.JobApplicationConst.JobApplicationNotFound;
                return result;
            }

            JobApplication jobAppAlreadyHired = await _unitOfWork.JobApplicationRepository.GetFirstOrDefault(j => j.StudentId == jobApplication.StudentId && j.ApplyStatus == (int)JobApplicationEnum.Hired && j.IsDeleted == false)!;

            if (applyStatus == (int)JobApplicationEnum.Hired)
            {
                if (jobAppAlreadyHired != null)
                {
                    result.Message = Consts.JobApplicationConst.AlreadyHiredStudent;
                    return result;
                }
            }


            jobApplication.ApplyStatus = applyStatus;
            jobApplication.ModifiedAt = DateTime.Now;
            jobApplication.ModifiedBy = userAccount.Id;

            GenericReturnDTO genericReturn = _unitOfWork.JobApplicationRepository.Update(jobApplication);

            await _unitOfWork.SaveChangesAsync();

            result.Success = Consts.GeneralConst.StatusSuccess;
            result.Message = Consts.JobApplicationConst.JobApplicationUpdateSuccessfully;

            return result;
        }
        catch (Exception)
        {
            result.Message = Consts.JobApplicationConst.JobApplicationNotUpdated;

            return result;
        }
    }

    public async Task<ApiResponseDTO<List<int>>> GetJobApplicationsByStudentForCompanyId(int companyId)
    {
        TokenDataDTO tokenData = _globalService.GetTokenData();
        List<JobApplication> listings = await _unitOfWork.JobApplicationRepository.GetListInclude(j => j.StudentId == tokenData.UserId && j.CompanyListing.CompanyId == companyId, j => j.CompanyListing, j => j.CompanyListing.Company);
        List<int> listingIds = listings.Select(li => li.CompanyListingId).ToList();

        return new ApiResponseDTO<List<int>>(Consts.GeneralConst.StatusSuccess, "Data fetched successfully", listingIds);
    }
}
