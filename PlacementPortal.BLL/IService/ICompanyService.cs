using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface ICompanyService
{
 
  Task<PaginationDTO<CompanyProfileDTO>> GetCompanyProfilesPaginated(PaginationRequestDTO paginationRequest, JobFiltersDTO jobFilters);

  Task<GenericReturnDTO> AddCompanyProfile(CompanyProfileViewModel companyProfile);

  Task<OperationStatusDTO> UpdateCompanyProfile(CompanyProfileViewModel companyProfile, int companyId);

  Task<GenericReturnDTO> UpdateJobListings(JobListingOperationDTO jobListingData);

  Task<CompanyProfileViewModel> GetCompanyDetailById(int companyId);

  Task<OperationStatusDTO> DeleteCompanyProfile(int companyId);

  Task<PaginationDTO<CompanyJobListingDTO>> GetCompanyJobListingsPaginated(int companyId, PaginationRequestDTO paginationRequest, JobFiltersDTO jobFilters);

  Task<List<CompanyJobListingDTO>> GetCompanyJobListings(string companyId);

  Task<List<CompanyProfile>> GetJobsClosingWithin24HoursAsync();

  Task<GenericReturnDTO> GetDistinctJobProfiles();

  string ProtectId(int id);

  int UnProtectId(string id);

  
}
