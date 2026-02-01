using Microsoft.AspNetCore.Mvc;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.IService;
using PlacementPortal.BLL.Services;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.Web.Controllers;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class CompanyController : Controller
{
  private readonly ICompanyService _companyService;

  public CompanyController(ICompanyService companyService)
  {
    _companyService = companyService;
  }

  [CustomAuthorize(new string[] { "Student", "TPO" }, "/Company/Company")]
  [HttpGet]
  public IActionResult Company()
  {
    return View();
  }

  [CustomAuthorize(new string[] { "TPO" })]
  public async Task<IActionResult> CompanyProfile(int id)
  {
    try
    {
      CompanyProfileViewModel companyProfile = await _companyService.GetCompanyDetailById(id);
      return View("AddCompanyProfile", companyProfile);
    }
    catch (Exception)
    {
      return View("Company");
    }
  }

  [CustomAuthorize(new string[] { "TPO" })]
  public IActionResult JobListings(int companyId)
  {
    return View("JobListingForm");
  }

  [HttpGet]
  public IActionResult GetCompanyId(int companyId)
  {
    try
    {
      string companyIdEncrypted = _companyService.ProtectId(companyId);
      return Json(new ApiResponseDTO<string>(Consts.GeneralConst.StatusSuccess, Consts.HashForm.Converted, companyIdEncrypted));
    }
    catch (Exception)
    {

      return Json(new ApiResponseDTO<string>(Consts.GeneralConst.StatusFailed, Consts.HashForm.NotConverted));
    }
  }

  [HttpGet]
  public IActionResult RenderCompaniesList()
  {
    return PartialView("_CompaniesList");
  }

  [HttpPost]
  public async Task<IActionResult> AddCompanyProfile(CompanyProfileViewModel companyProfile)
  {
    try
    {
      GenericReturnDTO result = await _companyService.AddCompanyProfile(companyProfile);

      return Json(new ApiResponseDTO<string>(result.Success, result.Message, data: result.Data! as string));
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.CompanyConst.CompanyProfileFailedToAdd));
    }
  }

  [HttpPost]
  public async Task<IActionResult> UpdateCompanyProfile(CompanyProfileViewModel companyProfile, int companyId)
  {
    try
    {
      OperationStatusDTO result = await _companyService.UpdateCompanyProfile(companyProfile, companyId);

      return Json(new ApiResponseDTO<string>(result.Success, result.Message));
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.CompanyConst.CompanyProfileFailedToUpdate));
    }
  }

  [HttpPost]
  public async Task<IActionResult> DeleteCompanyProfile(int companyId)
  {
    try
    {
      OperationStatusDTO operationStatus = await _companyService.DeleteCompanyProfile(companyId);
      return Json(new ApiResponseDTO<string>(operationStatus.Success, operationStatus.Message, companyId.ToString()));
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.CompanyConst.CompanyNotDeleted));
    }
  }

  [HttpPost]
  public async Task<IActionResult> AddJobListingsEnhanced(JobListingOperationDTO jobListing)
  {
    try
    {
      GenericReturnDTO result = await _companyService.UpdateJobListings(jobListing);
      return Json(new ApiResponseDTO<string>(result.Success, result.Message, result.Data?.ToString()));
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.CompanyConst.JobListingFailedToSave));
    }
  }

  [HttpGet]
  public async Task<IActionResult> RenderUpdateCompanyProfile(int companyId)
  {
    CompanyProfileViewModel companyData = await _companyService.GetCompanyDetailById(companyId);
    return PartialView("_AddCompanyProfile", companyData);
  }

  [HttpPost]
  public async Task<IActionResult> GetJobListings(string companyId)
  {
    List<CompanyJobListingDTO> data = await _companyService.GetCompanyJobListings(companyId);
    return Json(new { data });
  }

  [HttpPost]
  public async Task<IActionResult> GetCompaniesList(PaginationRequestDTO paginationRequest, JobFiltersDTO jobSearchFilters)
  {
    try
    {
      PaginationDTO<CompanyProfileDTO>? CompanyProfiles = await _companyService.GetCompanyProfilesPaginated(paginationRequest, jobSearchFilters);
      return PartialView("_CompaniesList", CompanyProfiles);
    }
    catch (Exception)
    {
      return Json(new ApiResponseDTO<string>(false, GeneralConst.ErrorFetchingDetails));
      throw new Exception(Consts.CompanyConst.CompanyFetchingFailed);
    }
  }

  [HttpPost]
  public async Task<IActionResult> GetJobListingsByCompany(int companyId, PaginationRequestDTO paginationRequest, JobFiltersDTO jobSearchFilters)
  {
    try
    {
      PaginationDTO<CompanyJobListingDTO>? companyJobListings = await _companyService.GetCompanyJobListingsPaginated(companyId, paginationRequest, jobSearchFilters);
      return PartialView("_CompanyJobListings", companyJobListings);
    }
    catch (Exception)
    {
      throw new Exception(Consts.CompanyConst.ListingsFetchingFailed);
    }
  }

  [HttpGet]
  public async Task<IActionResult> GetDistinctJobProfiles()
  {
    try
    {
      GenericReturnDTO data = await _companyService.GetDistinctJobProfiles();
      return Json(new ApiResponseDTO<List<string>>(data.Success, data.Message, data.Data as List<string> ?? new List<string>()));
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.GeneralConst.ErrorFetchingData));
    }
  }
}

