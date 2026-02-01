using Microsoft.AspNetCore.Mvc;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.IService;
using PlacementPortal.BLL.Services;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.Web.Controllers;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class JobApplicationController : Controller
{
    private readonly IJobApplicationService _jobApplicationService;

    public JobApplicationController(IJobApplicationService jobApplicationService)
    {
        _jobApplicationService = jobApplicationService;
    }

    [CustomAuthorize(new string[] { "TPO" })]
    [HttpGet]
    public IActionResult ViewApplications()
    {
        return View("JobApplications");
    }

    [CustomAuthorize(new string[] { "Student" })]
    [HttpGet]
    public IActionResult ViewApplicationsByStudent()
    {
        return View("StudentApplication");
    }

    [HttpPost]
    public async Task<IActionResult> ApplyForJob(int jobListingId)
    {
        try
        {
            OperationStatusDTO result = await _jobApplicationService.SubmitJobApplicationAsync(jobListingId);
            return Json(new ApiResponseDTO<string>(result.Success, result.Message!));
        }
        catch (Exception ex)
        {
            if (ex.Message == CustomExceptionConst.StudentRegistrationInComplete)
            {
                return RedirectToAction("Student", "Student");
            }

            return Json(ApiResponse.Fail<string>(Consts.JobApplicationConst.FailedToApply));
        }
    }

    [HttpGet]
    public async Task<IActionResult> ViewApplicationByStudentPaginated(PaginationRequestDTO paginationRequest)
    {
        try
        {
            PaginationDTO<StudentJobApplicationDTO> jobApplications = await _jobApplicationService.GetPersonalApplicationsPaginatedEnhanced(paginationRequest);
            return PartialView("_JobApplicationByStudent", jobApplications);
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.JobApplicationConst.FailedToLoadApplications));
        }
    }

    [HttpGet]
    public async Task<IActionResult> ViewApplicationsPaginated(PaginationRequestDTO paginationRequest, int statusFilter)
    {
        try
        {
            PaginationDTO<StudentJobApplicationDTO> jobApplications = await _jobApplicationService.GetApplicationsForTpoPaginated(paginationRequest, statusFilter);
            return PartialView("_JobApplicationsPaginated", jobApplications);
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.JobApplicationConst.FailedToLoadApplications));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetJobApplicationDetails(int applicationId)
    {
        try
        {
            JobApplicationDataDTO applicationData = await _jobApplicationService.GetApplicationDetails(applicationId);
            return PartialView("_ApplicationDetails", applicationData);
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.JobApplicationConst.FailedToLoadApplicationDetails));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetJobApplicationDetailsJson(int applicationId)
    {
        try
        {
            JobApplicationDataDTO applicationData = await _jobApplicationService.GetApplicationDetails(applicationId);
            return Json(new ApiResponseDTO<Object>(Consts.GeneralConst.StatusSuccess, Consts.GeneralConst.DateFetched, applicationData));
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.GeneralConst.ErrorFetchingData));
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateJobApplicationStatus(int applicationId, int applyStatus)
    {
        try
        {
            OperationStatusDTO result = await _jobApplicationService.UpdateApplicationStatus(applicationId, applyStatus);
            return Json(new ApiResponseDTO<string>(result.Success, result.Message));
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.JobApplicationConst.JobApplicationNotUpdated));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetJobApplicationsByCompanyAndStudent(int companyId)
    {
        try
        {
            ApiResponseDTO<List<int>> apiResponse = await _jobApplicationService.GetJobApplicationsByStudentForCompanyId(companyId);

            return Json(apiResponse);
        }
        catch(Exception)
        {
            return Json(ApiResponse.Fail<string>("Failed to retrive application data for students"));
        }


    }

}

